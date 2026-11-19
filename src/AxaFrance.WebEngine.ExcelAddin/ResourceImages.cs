// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using System.Reflection;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Loads embedded browser images without keeping the resource stream open.
    /// </summary>
    internal static class ResourceImages
    {
        internal static Bitmap? Load(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"AxaFrance.WebEngine.ExcelAddin.Resources.{fileName}";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return null;
            }

            using var image = Image.FromStream(stream);
            return new Bitmap(image);
        }
    }
}
