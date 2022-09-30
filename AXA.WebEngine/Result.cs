// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace AXA.WebEngine
{
    /// <summary>
    /// Represent the result of a TestStep or an Action
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// The default value, while the test item has not been executed.
        /// </summary>
        None,

        /// <summary>
        /// The result is Passed
        /// </summary>
        Passed,

        /// <summary>
        /// The result is failed, for any functional and technical causes.
        /// </summary>
        Failed,

        /// <summary>
        /// <para>The action encounters a Critical Error.</para>
        /// <para>If an Action or Test Step encounters Critical Error, the current test will stop running. Test Framework will try to run next test cases. If the test case is failed with Critical Error, the whole Test Suite will stop running.</para>
        /// </summary>
        CriticalError,

        /// <summary>
        /// Steps are ignored due to flow controller (Intendly stopped at some points)
        /// </summary>
        Ignored,
    }
}
