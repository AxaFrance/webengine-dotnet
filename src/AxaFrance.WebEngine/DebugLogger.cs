// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Text;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// Log the content to both Console and Debug (showing in visual studio)
    /// </summary>
    public static class DebugLogger
    {

        internal static StringBuilder SystemOutput { get; } = new StringBuilder();
        internal static StringBuilder SystemError { get; } = new StringBuilder();
        /// <summary>
        /// Writes a message in both Console and Debug view of visual studio.
        /// </summary>
        /// <param name="content">the content of the log to be written</param>
        /// <remarks>
        /// When developing test actions, use this method to ensure the log is written both on console and Debug view.
        /// <para>
        /// Messages written in console will be shown on screen and will be transmitted to the client side when using remote testing.
        /// Messages written on Debug view will allow 3rd test framework such as NUnit and/or Spec-flow to capture the test result in the report.
        /// </para>
        /// </remarks>
        public static void WriteLine(string content)
        {
            SystemOutput.AppendLine(content);
            System.Diagnostics.Debug.WriteLine(content);
            Console.WriteLine(content);
        }

        /// <summary>
        /// Writes a Warning message on the console with Yellow color. Warning is not considered as Error.
        /// </summary>
        /// <param name="content"></param>
        public static void WriteWarning(string content)
        {
            SystemOutput.AppendLine(content);
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(content);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a message in both Console and Debug view of visual studio.
        /// </summary>
        /// <param name="content">the content of the log to be written</param>
        /// <remarks>
        /// When developing test actions, use this method to ensure the log is written both on console and Debug view.
        /// <para>
        /// Messages written in console will be shown on screen and will be transmitted to the client side when using remote testing.
        /// Messages written on Debug view will allow 3rd test framework such as NUnit and/or Spec-flow to capture the test result in the report.
        /// </para>
        /// </remarks>
        public static void WriteLine(StringBuilder content)
        {
            SystemOutput.AppendLine(content.ToString());
            System.Diagnostics.Debug.WriteLine(content);
            Console.WriteLine(content);
        }

        /// <summary>
        /// Writes a message in both Console and Debug view of visual Studio. in Red Text
        /// </summary>
        /// <param name="content">the content of the log to be written</param>
        /// <remarks>
        /// <para>
        /// Messages written in console will be shown on screen and will be transmitted to the client side when using remote testing.
        /// Messages written on Debug view will allow 3rd test framework such as NUnit and/or Spec-flow to capture the test result in the report.
        /// The Error message are writtin in Red color, but not in ErrorStream. Some CI Pipe line consider technical errors and stops the step from running if any messages are writen in Error Stream.
        /// </para>
        /// </remarks>
        public static void WriteError(string content)
        {
            SystemError.AppendLine(content);
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(content);
            Console.ResetColor();
        }
    }
}
