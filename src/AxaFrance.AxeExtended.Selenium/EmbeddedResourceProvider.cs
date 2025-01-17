using Deque.AxeCore.Selenium;
using System.IO;
using System.Text;

namespace AxaFrance.AxeExtended.Selenium
{
    internal static class EmbeddedResourceProvider
    {
        public static string ReadEmbeddedFile(string fileName)
        {
            var assembly = typeof(AxeBuilder).Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"Deque.AxeCore.Selenium.Resources.{fileName}");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
