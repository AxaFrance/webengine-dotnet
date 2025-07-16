// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// Determine browser type to run the tests. Browser Type is limited according to the platform.
    /// </summary>
    [Serializable]
    public enum BrowserType
    {

        /// <summary>
        /// Mozilla Firefox
        /// </summary>
        Firefox = 1,

        /// <summary>
        /// Google Chrome (Desktop or Mobile)
        /// </summary>
        Chrome,

        /// <summary>
        /// Microsoft Edge (Chromium based)
        /// </summary>
        ChromiumEdge,

        /// <summary>
        /// IOS Application. The application package must be provided via AppId parameter.
        /// </summary>
        IOSNative,

        /// <summary>
        /// Android Application.The application package must be provided via AppId parameter.
        /// </summary>
        AndroidNative,

        /// <summary>
        /// Test will be run on Safari browser onder MacOS or iOS device.
        /// </summary>
        Safari
    }
}
