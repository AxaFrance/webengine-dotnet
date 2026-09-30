namespace AxaFrance.WebEngine.Cli;

internal static class Program
{
    public static Task<int> Main(string[] args)
    {
        var options = CliOptions.Parse(args);
        return CliRunner.RunAsync(options);
    }
}
