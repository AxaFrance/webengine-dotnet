// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-19 12:02
using System;
using System.Linq;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// The data for a single test, identified with it's name.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class TestData
    {
        /// <summary>
        /// The Name of the test case. It is used as a Key to find reladed testdata for a test case.
        /// TestName should be unique, or only the first matching data will be selected
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// The test data, provided as a list of Name-Value pair
        /// </summary>
        public Variable[] Data { get; set; } = new Variable[0];

        /// <summary>
        /// Get the value of a test data.
        /// </summary>
        /// <param name="name">the name of the variable</param>
        /// <returns>Value of Parameter if the parameter.</returns>
        /// <remarks>
        /// If the given parameter does not exist, an Exception will be thrown.
        /// </remarks>
        public string GetValue(string name)
        {
            var param = Data.FirstOrDefault(x => x.Name == name);
            if (param != null)
            {
                return param.Value;
            }
            else
            {
                throw new WebEngineGeneralException($"Parameter {name} does not exist in the current Test data list (Test={TestName}).");
            }
        }

        /// <summary>
        /// Try gets the value of a test data.
        /// </summary>
        /// <param name="name">the name of the variable</param>
        /// <returns>Value of Parameter if the parameter.</returns>
        /// <remarks>
        /// If the given parameter does not exist, null will be returned.
        /// </remarks>
        public string TryGetValue(string name)
        {
            try
            {
                return GetValue(name);
            }
            catch (WebEngineGeneralException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Update a value of a parameter
        /// </summary>
        /// <param name="name">name of the parameter to update</param>
        /// <param name="value">value of the parameter</param>
        /// <remarks>If the variable does not exist in the test data, this function will do nothing.</remarks>
        public void SetValue(string name, string value)
        {
            var param = Data.FirstOrDefault(x => x.Name == name);
            if (param != null)
            {
                param.Value = value;
            }
        }
    }
}
