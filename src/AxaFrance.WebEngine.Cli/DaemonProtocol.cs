using System.Text.Json;

namespace AxaFrance.WebEngine.Cli;

internal static class DaemonProtocol
{
    public const string Version = "1";

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static DaemonResponse Success<T>(string requestId, T result)
        => new(requestId, true, JsonSerializer.SerializeToElement(result, JsonOptions), null);

    public static DaemonResponse Error(string requestId, string code, string message)
        => new(requestId, false, null, new DaemonError(code, message));
}

internal sealed record DaemonRequest(
    string RequestId,
    string Command,
    JsonElement? Arguments = null);

internal sealed record DaemonResponse(
    string RequestId,
    bool Ok,
    JsonElement? Result,
    DaemonError? Error);

internal sealed record DaemonError(string Code, string Message);

internal sealed record DaemonStatus(
    string ProtocolVersion,
    int ProcessId,
    DateTimeOffset StartedAtUtc,
    string PipeName);
