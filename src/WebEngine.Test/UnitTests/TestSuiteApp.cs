// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using System.Collections.Generic;

namespace WebEngine.Test.UnitTests
{
    public class TestSuiteApp : AxaFrance.WebEngine.MobileApp.TestSuiteApp
    {
        public override KeyValuePair<string, TestCase>[] TestCases
        {
            get
            {
                var testCases = new KeyValuePair<string, TestCase>[]
                {
                    new KeyValuePair<string, TestCase>("Addition1", new TestCaseMobileApp())
                };

                return testCases;
            }
        }
    }
}
