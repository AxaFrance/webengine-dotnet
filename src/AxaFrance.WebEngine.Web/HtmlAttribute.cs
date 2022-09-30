// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// Describes an attribute of HTML tag. Use <see cref="HtmlAttribute"/> to describe a non-standand html attributes.
    /// </summary>
    public class HtmlAttribute
    {
        /// <summary>
        /// The name of the html attribute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the html attribute
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates an instance of Html Attribute
        /// </summary>
        public HtmlAttribute() { }

        /// <summary>
        /// Creates an instance of Html Attribute with name initialized.
        /// </summary>

        public HtmlAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates an instance of Html Attribute with name and value initialized.
        /// </summary>

        public HtmlAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
