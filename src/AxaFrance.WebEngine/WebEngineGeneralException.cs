// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// The generic exception thrown by the framework web engine;
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class WebEngineGeneralException : Exception
    {
        /// <summary>
        /// Initializing a new instance of the <see cref="WebEngineGeneralException"/>
        /// </summary>
        public WebEngineGeneralException()
        {

        }

        /// <summary>
        /// Initializing a new instance of the <see cref="WebEngineGeneralException"/> with customized error message.
        /// </summary>
        public WebEngineGeneralException(string message) : base(message)
        {

        }

        /// <summary>
        /// Initializing a new instance of the <see cref="WebEngineGeneralException"/> with customized error message and inner exception.
        /// </summary>
        public WebEngineGeneralException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
