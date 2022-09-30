// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// A Variable contains a name and it's value.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public sealed class Variable
    {
        /// <summary>
        /// Name of the variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the variable
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Get the Value of an variable from a list by its name
        /// </summary>
        /// <param name="source">The list of variable to be checked.</param>
        /// <param name="name">The name of the variable where its value should be retrieved.</param>
        /// <returns>The value of variable there its name is [name].</returns>
        /// <exception cref="ArgumentNullException">source is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
        public static string GetValue(IEnumerable<Variable> source, string name)
        {
            return source.First(x => x.Name == name).Value;
        }

        /// <summary>
        /// Initialize a new instance of Variable class
        /// </summary>
        public Variable()
        {

        }

        /// <summary>
        /// Initialize the variable with Name, and Value = null
        /// </summary>
        /// <param name="Name">the name of the variable</param>
        public Variable(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Initialize the varialbe with Name and Value
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public Variable(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Returns the string representation of the current Variable
        /// </summary>
        /// <returns>String representation of variable</returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, Value: {1}", Name, Value);
        }
    }
}
