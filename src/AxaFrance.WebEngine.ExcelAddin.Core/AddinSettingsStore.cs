// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Xml;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Loads and saves <see cref="AddinSettings"/> from the user profile.
    /// </summary>
    public sealed class AddinSettingsStore
    {
        /// <summary>
        /// Default settings file, kept at the same location as the former VSTO add-in.
        /// </summary>
        public static string DefaultPath { get; } = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AxaFrance.WebEngine",
            "excelSetting.xml");

        private readonly string _path;

        /// <summary>
        /// Creates a store for the given file path.
        /// </summary>
        public AddinSettingsStore(string? path = null)
        {
            _path = string.IsNullOrWhiteSpace(path) ? DefaultPath : path;
        }

        /// <summary>
        /// Settings file path.
        /// </summary>
        public string Path => _path;

        /// <summary>
        /// Loads settings, or returns defaults when the file does not exist or is invalid.
        /// </summary>
        public AddinSettings Load()
        {
            if (!File.Exists(_path))
            {
                return new AddinSettings();
            }

            try
            {
                using var stream = File.OpenRead(_path);
                using var reader = XmlReader.Create(stream, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                });
                var serializer = new XmlSerializer(typeof(AddinSettings));
                return serializer.Deserialize(reader) as AddinSettings ?? new AddinSettings();
            }
            catch (Exception)
            {
                // A broken settings file must not prevent the add-in from loading.
                return new AddinSettings();
            }
        }

        /// <summary>
        /// Saves settings atomically.
        /// </summary>
        public void Save(AddinSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            var directory = System.IO.Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempPath = _path + ".tmp";
            var serializer = new XmlSerializer(typeof(AddinSettings));
            using (var stream = File.Create(tempPath))
            {
                serializer.Serialize(stream, settings);
            }

            File.Move(tempPath, _path, overwrite: true);
        }
    }
}
