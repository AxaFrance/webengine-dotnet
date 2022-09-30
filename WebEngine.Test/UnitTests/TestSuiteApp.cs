// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AXA.WebEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    public class TestSuiteApp : AXA.WebEngine.MobileApp.TestSuiteApp
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
