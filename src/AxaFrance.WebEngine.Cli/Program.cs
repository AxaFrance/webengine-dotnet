namespace AxaFrance.WebEngine.Cli;

internal static class Program
{
    public static Task<int> Main(string[] args)
    {
        if (args.Length == 0)
            return CliRunner.RunShellAsync(new ShellOptions());

        if (args[0].Equals("shell", StringComparison.OrdinalIgnoreCase))
        {
            var explicitShellOptions = TryParseShellOptions(args[1..]);
            if (explicitShellOptions is null)
            {
                Console.Error.WriteLine(
                    "Shell options may only be --json, --quiet, and --pipe <name>.");
                return Task.FromResult(2);
            }

            return CliRunner.RunShellAsync(explicitShellOptions);
        }

        if (args[0].Equals("-c", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length == 1)
            {
                Console.Error.WriteLine("The -c option requires a command string.");
                return Task.FromResult(2);
            }

            return CliRunner.RunCommandLineAsync(string.Join(' ', args[1..]));
        }

        if (TryParseShellOptions(args) is { } shellOptions)
            return CliRunner.RunShellAsync(shellOptions);

        var options = CliOptions.Parse(args);
        return CliRunner.RunAsync(options);
    }

    private static ShellOptions? TryParseShellOptions(IReadOnlyList<string> args)
    {
        var json = false;
        var quiet = false;
        var pipeName = DaemonEndpoint.DefaultPipeName;

        for (var index = 0; index < args.Count; index++)
        {
            switch (args[index].ToLowerInvariant())
            {
                case "--json":
                    json = true;
                    break;
                case "--quiet":
                    quiet = true;
                    break;
                case "--pipe":
                    if (index + 1 >= args.Count || string.IsNullOrWhiteSpace(args[++index]))
                        return null;

                    pipeName = args[index];
                    break;
                default:
                    return null;
            }
        }

        return new ShellOptions(json, quiet, pipeName);
    }
}

internal sealed record ShellOptions(
    bool Json = false,
    bool Quiet = false,
    string? PipeName = null);
