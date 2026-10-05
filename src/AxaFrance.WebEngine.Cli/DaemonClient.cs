using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text.Json;

namespace AxaFrance.WebEngine.Cli;

internal sealed class DaemonClient : IDisposable
{
    private readonly string _pipeName;
    private readonly bool _keepConnection;
    private DaemonConnection? _connection;

    public DaemonClient(string pipeName, bool keepConnection = false)
    {
        _pipeName = pipeName;
        _keepConnection = keepConnection;
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
        if (_keepConnection)
        {
            var connection = _connection
                ??= await DaemonConnection.ConnectAsync(_pipeName, timeout, cancellationToken);
            try
            {
                return await connection.RequestAsync(command, arguments, timeout, cancellationToken);
            }
            catch (Exception ex) when (
                ex is IOException
                or InvalidDataException
                or TimeoutException
                or ObjectDisposedException)
            {
                ResetConnection();
                throw;
            }
        }

        return await DaemonConnection.RequestOnceAsync(
            _pipeName,
            command,
            arguments,
            timeout,
            cancellationToken);
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
        catch (ObjectDisposedException)
        {
            return null;
        }
    }

    public void ResetConnection()
    {
        _connection?.Dispose();
        _connection = null;
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

    public void Dispose()
    {
        ResetConnection();
    }
}

internal sealed class DaemonConnection : IDisposable
{
    private readonly NamedPipeClientStream _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    private DaemonConnection(NamedPipeClientStream client)
    {
        _client = client;
        _reader = new StreamReader(client);
        _writer = new StreamWriter(client) { AutoFlush = true };
    }

    public static async Task<DaemonConnection> ConnectAsync(
        string pipeName,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(timeout);

        var client = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);

        try
        {
            await client.ConnectAsync(timeoutCancellation.Token);
            return new DaemonConnection(client);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            client.Dispose();
            throw new TimeoutException(
                $"The WebEngine daemon did not respond within {timeout.TotalMilliseconds:0} ms.",
                ex);
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    public static async Task<DaemonResponse> RequestOnceAsync(
        string pipeName,
        string command,
        JsonElement? arguments,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var connection = await ConnectAsync(pipeName, timeout, cancellationToken);
        return await connection.RequestAsync(command, arguments, timeout, cancellationToken);
    }

    public async Task<DaemonResponse> RequestAsync(
        string command,
        JsonElement? arguments,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCancellation.CancelAfter(timeout);

        var request = new DaemonRequest(Guid.NewGuid().ToString("N"), command, arguments);
        await _writer.WriteLineAsync(JsonSerializer.Serialize(request, DaemonProtocol.JsonOptions));

        try
        {
            var line = await _reader.ReadLineAsync(timeoutCancellation.Token);
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

    public void Dispose()
    {
        _writer.Dispose();
        _reader.Dispose();
        _client.Dispose();
    }
}
