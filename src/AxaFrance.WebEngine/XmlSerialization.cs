// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// XML deserialization helpers that disable DTD processing and external resolvers.
    /// </summary>
    internal static class XmlSerialization
    {
        private static XmlReaderSettings CreateSecureSettings()
        {
            return new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
        }

        internal static T Deserialize<T>(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(stream, CreateSecureSettings()))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        internal static T Deserialize<T>(TextReader textReader)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(textReader, CreateSecureSettings()))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
