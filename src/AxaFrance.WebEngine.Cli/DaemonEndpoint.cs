using System.Security.Cryptography;
using System.Text;

namespace AxaFrance.WebEngine.Cli;

internal static class DaemonEndpoint
{
    private const string PipePrefix = "webengine-daemon-";

    public static string DefaultPipeName
    {
        get
        {
            var identity = $"{Environment.UserDomainName}\\{Environment.UserName}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(identity));
            return PipePrefix + Convert.ToHexString(hash)[..12].ToLowerInvariant();
        }
    }
}
