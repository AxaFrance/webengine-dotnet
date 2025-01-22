// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace WebEngine.Test.UnitTests
{
    public class TestCaseMobileApp : AxaFrance.WebEngine.MobileApp.TestCaseApp
    {
        public TestCaseMobileApp()
        {
            TestSteps = new AxaFrance.WebEngine.TestStep[]
            {
                new AxaFrance.WebEngine.TestStep{ Action = "MobileAppStep1" },
                new AxaFrance.WebEngine.TestStep{ Action = "MobileAppStep2" },
                new AxaFrance.WebEngine.TestStep{ Action = "MobileAppStep3" },
                new AxaFrance.WebEngine.TestStep{ Action = "MobileAppStep4" }
            };
        }
    }
}
