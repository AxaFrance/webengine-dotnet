using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;

namespace AxaFrance.WebEngine.Cli;

internal sealed class DaemonClient
{
    private readonly string _pipeName;

    public DaemonClient(string pipeName)
    {
        _pipeName = pipeName;
    }

    public async Task<DaemonResponse> RequestAsync(
        string command,
        TimeSpan timeout,
        CancellationToken cancellationToken)
        => await RequestAsync(command, null, timeout, cancellationToken);

    public async Task<DaemonResponse> RequestAsync(
        string command,
        JsonElement? arguments,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(timeout);

        using var client = new NamedPipeClientStream(
            ".",
            _pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        try
        {
            await client.ConnectAsync(timeoutCancellation.Token);

            using var reader = new StreamReader(client);
            using var writer = new StreamWriter(client) { AutoFlush = true };

            var request = new DaemonRequest(Guid.NewGuid().ToString("N"), command, arguments);
            await writer.WriteLineAsync(JsonSerializer.Serialize(request, DaemonProtocol.JsonOptions));

            var line = await reader.ReadLineAsync(timeoutCancellation.Token);
            if (line is null)
                throw new IOException("The WebEngine daemon closed the connection without a response.");

            var response = JsonSerializer.Deserialize<DaemonResponse>(line, DaemonProtocol.JsonOptions);
            return response
                ?? throw new InvalidDataException("The WebEngine daemon returned an empty response.");
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"The WebEngine daemon did not respond within {timeout.TotalMilliseconds:0} ms.",
                ex);
        }
    }

    public async Task<DaemonResponse> StartAsync(CancellationToken cancellationToken)
    {
        var current = await TryPingAsync(cancellationToken);
        if (current is not null)
            return current;

        using var process = StartDaemonProcess();
        var deadline = DateTime.UtcNow.AddSeconds(10);

        while (DateTime.UtcNow < deadline)
        {
            if (process.HasExited)
                throw new InvalidOperationException(
                    $"The WebEngine daemon exited during startup with code {process.ExitCode}.");

            var response = await TryPingAsync(cancellationToken);
            if (response is not null)
                return response;

            await Task.Delay(100, cancellationToken);
        }

        throw new TimeoutException("The WebEngine daemon did not become ready within 10 seconds.");
    }

    public async Task<DaemonResponse?> TryPingAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await RequestAsync("daemon.ping", TimeSpan.FromMilliseconds(500), cancellationToken);
        }
        catch (TimeoutException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    private Process StartDaemonProcess()
    {
        var processPath = Environment.ProcessPath
            ?? throw new InvalidOperationException("The current process path is unavailable.");
        var useShellExecute = OperatingSystem.IsWindows();
        var startInfo = new ProcessStartInfo
        {
            FileName = processPath,
            UseShellExecute = useShellExecute,
            CreateNoWindow = !useShellExecute,
            WindowStyle = useShellExecute
                ? ProcessWindowStyle.Hidden
                : ProcessWindowStyle.Normal,
            RedirectStandardInput = !useShellExecute,
            RedirectStandardOutput = !useShellExecute,
            RedirectStandardError = !useShellExecute
        };

        if (Path.GetFileNameWithoutExtension(processPath)
            .Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrWhiteSpace(assemblyPath))
                throw new InvalidOperationException("The CLI assembly path is unavailable.");

            startInfo.ArgumentList.Add(assemblyPath);
        }

        startInfo.ArgumentList.Add("daemon");
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--quiet");
        startInfo.ArgumentList.Add("--pipe");
        startInfo.ArgumentList.Add(_pipeName);

        var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The WebEngine daemon process could not be started.");

        if (!useShellExecute)
        {
            process.StandardInput.Close();
            _ = DrainAsync(process.StandardOutput);
            _ = DrainAsync(process.StandardError);
        }

        return process;
    }

    private static async Task DrainAsync(StreamReader reader)
    {
        try
        {
            await reader.ReadToEndAsync();
        }
        finally
        {
            reader.Dispose();
        }
    }
}
