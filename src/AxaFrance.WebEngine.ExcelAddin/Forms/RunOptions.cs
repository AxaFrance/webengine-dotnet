// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Options selected in the run dialog.
    /// </summary>
    internal sealed class RunOptions
    {
        public BrowserType Browser { get; init; }

        public string? Platform { get; init; }

        public string? Device { get; init; }

        public string? AppId { get; init; }

        public bool ManualMode { get; init; }

        public bool ShowReport { get; init; }
    }
}
