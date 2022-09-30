// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Text;
using System.Xml.Serialization;

namespace AXA.WebEngine
{
    /// <summary>
    /// A step within a test case. 
    /// 
    /// This class for internal use for reflection.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class TestStep
    {
        /// <summary>
        /// The qualified name of class to be called when running an Action.
        /// the class name must be derived from SharedActionBase
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The result of the teststep. the default value is None;
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestStep()
        {
            Result = WebEngine.Result.None;
        }

        /// <summary>
        /// the details of the test result.
        /// </summary>
        public StringBuilder Details { get; internal set; } = new StringBuilder();

    }
}
