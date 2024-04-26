using Deque.AxeCore.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.Selenium
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
