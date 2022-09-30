// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// This class defines the structure of environment variables
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class EnvironmentVariables
    {

        /// <summary>
        /// List of variables
        /// </summary>
        [XmlElement("Variable")]
        public List<Variable> Variables { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public EnvironmentVariables()
        {
            Variables = new List<Variable>();
        }

        internal static EnvironmentVariables _instance { get; set; } = null;

        /// <summary>
        /// Environment Variables currently loaded. If variables are not loaded by calling LoadFrom() method, it will returns null.
        /// <para>Attention: if there are already variables loaded and LoadFrom method are called again. all variables are cleaned before the second call is done </para>
        /// </summary>
        public static EnvironmentVariables Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EnvironmentVariables();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Load the environment variables from an XML file.
        /// <para>the path should be full path</para>
        /// <para>UNC path is supported: \\netshare\path\somefile.xml</para>
        /// <para>Web path is NOT supported. e.g.: http://somewhere.com/somefile.xml </para>
        /// Make sure that the automation solution has the access rights to the file, or an exception will be thrown.
        /// </summary>
        /// <param name="xmlFile">The path of the XML file </param>
        public static void LoadFrom(string xmlFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EnvironmentVariables));
            using (StreamReader sr = new StreamReader(xmlFile))
            {
                _instance = (EnvironmentVariables)serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Load the environment variables from an Stream
        /// the stream data is in XML format
        /// </summary>
        /// <param name="stream">the stream which the environment variables are stored</param>
        public static void LoadFrom(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestSuiteData));
            _instance = (EnvironmentVariables)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Returns the value of the environment variable by its given name.
        /// </summary>
        /// <param name="name">Name of the variable to find.</param>
        /// <returns>Value of the variable</returns>
        public string GetValue(string name)
        {
            if (this.Variables == null)
            {
                DebugLogger.WriteLine("environmentVarialbe not initialized");
            }
            else if (!this.Variables.Exists(x => x.Name == name))
            {
                DebugLogger.WriteLine(string.Format("EnvironmentVariable {0} not found", name));
            }
            else
            {
                string value = this.Variables.Find(x => x.Name == name).Value;
                while (value.StartsWith("$"))
                {
                    value = this.Variables.Find(x => x.Name == value.Substring(1)).Value;
                }
                return value;
            }

            throw new WebEngineGeneralException(string.Format("Impossible to get value of variable {0}", name));
        }
    }
}
