// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-19 12:02
using AxaFrance.WebEngine.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// A TestCase contains one or several test steps, these steps will be run one after another.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public abstract class TestCase
    {
        private const int indentLength = 2;
        /// <summary>
        /// A flag marks ignoring all remaining test steps.
        /// </summary>
        internal static bool IgnoreAllSteps { get; set; }

        /// <summary>
        /// Test Information. to privide more detailed information during test execution.
        /// This information can be visualized in the test report.
        /// </summary>

        public StringBuilder Information { get; private set; } = new StringBuilder();

        /// <summary>
        /// The directory of the log of TestCase. The test script can use this directory to store personalised data.
        /// </summary>
        public string TestCaseLogDir { get; internal set; }

        /// <summary>
        /// The DateTime value which the Test case has been started.
        /// </summary>
        public static DateTime StartDate { get; protected set; }

        /// <summary>
        /// The context of the current test case, which stores variables and values to be shared between different actions within the same test case.
        /// </summary>
        public List<Variable> ContextValues { get; protected set; } = new List<Variable>();

        /// <summary>
        /// The test data (a list of named values) used for the current test case.
        /// Test Data is initialized from TestSuiteData by its name.
        /// </summary>
        public TestData TestData { get; internal set; } = new TestData();

        /// <summary>
        /// The overall result of the current Test Case. if any of step fails, then the overall test case is failed.
        /// </summary>
        public Result TestCaseResult { get; set; } = Result.None;

        /// <summary>
        /// [Optional] The uniqueId of the current test case. This id if provided can be used as a key to associate test case and the key in your test management system.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// The name of the test case. this name will be shown to users.
        /// <para>The Name is a required parameter to be assigned before the call of Run() or BeginRun() Method </para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The test steps to be run sequentially
        /// </summary>
        [XmlElement("TestStep")]
        public TestStep[] TestSteps { get; set; }

        /// <summary>
        /// The context object for the test case. For Web test using Selenium WebDriver, the Context will be an WebDrive object. 
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// Updates the information of test case execution to the associated TestCase Report.
        /// </summary>
        /// <returns></returns>
        public TestCaseReport GetReport()
        {
            testCaseReport.ContextValues = this.ContextValues;
            testCaseReport.Result = this.TestCaseResult;
            testCaseReport.TestData = new List<Variable>(this.TestData.Data);
            testCaseReport.Log = this.Information.ToString();
            foreach (var r in testCaseReport.ActionReports)
            {
                if (r.Result == Result.None)
                {
                    r.Result = Result.Passed;
                }
            }
            return testCaseReport;
        }

        /// <summary>
        /// This is the TestCase report which will be updated during test.
        /// </summary>
        protected TestCaseReport testCaseReport { get; set; }

        /// <summary>
        /// This method will be called to run the current Test. Run calls automatically Initialize and Cleanup.
        /// </summary>
        /// <param name="report">
        /// The testcase report object initiaized by the test framework. After the test execution, use <see cref="GetReport"/> method to get a valorized report.
        /// </param>
        /// <param name="Pause">
        /// <para>True: When a test case ends or error encounters, the test execution will be paused until user presses Enter key</para>
        /// <para>False: do not pause after each test (Default Value)</para>
        /// </param>
        /// <returns>The test Result of test case</returns>
        /// <remarks>When running automated test with IC or remote mode, do not assign Pause = True</remarks>
        public virtual Result Run(TestCaseReport report, bool Pause = false)
        {
            this.testCaseReport = report;
            SharedActionBase.Indent = indentLength;
            StartDate = DateTime.Now;
            this.TestCaseResult = Result.None;

            try
            {
                TestData = TestSuiteData.Current.GetTestData(Name);
                UniqueId = this.TestData.TryGetValue("UNIQUE_ID");
                if (UniqueId == null)
                {
                    UniqueId = Name;
                }
                DebugLogger.WriteLine($"[Test Case] Start running '{this.Name}'");
            }
            catch
            {
                DebugLogger.WriteError("[WARNING] No test data found for this test case. " + this.Name);
            }

            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLine(ex.ToString());
                this.TestCaseResult = Result.Failed;
                return Result.Failed;
            }

            //Pass all steps' result to Pending
            foreach (TestStep step in TestSteps)
            {
                step.Result = Result.None;
            }



            foreach (TestStep step in TestSteps)
            {
                ActionReport ar = new ActionReport();
                ar.Result = Result.None;
                report.ActionReports.Add(ar);

                if (IgnoreAllSteps)
                {
                    step.Result = Result.Ignored;
                    continue;
                }

                try
                {
                    string action = step.Action;
                    Type t = Utilities.GetTypeByClassName(action, this);
                    string name = Utilities.GetDescriptionByClassName(action);
                    ar.Name = name;
                    Variable[] parameters = TestData?.Data ?? new Variable[1] { new Variable { Name = "Empty", Value = string.Empty } };
                    var actionObj = SharedActionBase.DoAction(t, this, Context, ContextValues, ar, parameters);

                    ContextValues = actionObj.ContextValues;
                    bool result = true;
                    if (actionObj.ActionResult != Result.Ignored)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        DebugLogger.WriteLine("Run checkpoint:" + action);
                        Console.ResetColor();
                        result = actionObj.DoCheckpoint(Context);
                    }
                    else
                    {
                        step.Details.AppendLine("Checkpoint is not performed, because the status of the action is Ignored");
                    }
                    if (actionObj.Information != null)
                    {
                        step.Details.Append(actionObj.Information);
                    }

                    TranslateResult(result, actionObj, step);


                    ar.Log = actionObj.Information.ToString();
                    ar.Screenshots = actionObj.Screenshots;
                    ar.EndTime = DateTime.Now;
                    ar.ContextValues = actionObj.ContextValues;
                    ar.Result = step.Result;
                    SharedActionBase.Indent -= indentLength;
                }
                catch (ActionNotFoundException ae)
                {
                    string errorMessage = $"Can not found the test action: {step.Action} {ae.Message}";
                    ar.Log = errorMessage;
                    DebugLogger.WriteLine($"\t{errorMessage}");
                    step.Result = Result.CriticalError;
                    step.Details.AppendLine($"Fatal Error: {errorMessage}");
                    DebugLogger.WriteLine(step.Details);
                    ar.Result = Result.Failed;
                    TestCaseResult = Result.Failed;
                    break;
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteError("[ERROR]\tAn critical exception has thrown while running the test case:" + Name);
                    DebugLogger.WriteError(ex.ToString());
                    Information.AppendLine(ex.Message);
                    step.Result = Result.CriticalError;
                    step.Details.AppendLine($"Fatal Error: {ex.Message}");
                    ar.Result = Result.Failed;
                    TestCaseResult = Result.Failed;
                    break;
                }
            }


            if (TestCaseResult == Result.None)
            {
                TestCaseResult = Result.Passed;
            }
            DebugLogger.WriteLine("[DEBUG] TestCase result is " + TestCaseResult.ToString());
            if (Pause && (TestCaseResult == Result.Failed || TestCaseResult == Result.CriticalError))
            {
                DebugLogger.WriteLine("[DEBUG] press any key start clean up and run next test case.");
                Console.ReadKey();
            }

            DebugLogger.WriteLine("Cleaning Up");
            Thread cleanup = new Thread(new ThreadStart(() =>
            {
                Information.AppendLine(Cleanup());
            }));
            cleanup.Start();
            bool terminated = cleanup.Join(new TimeSpan(0, 2, 0));
            if (!terminated)
            {
                DebugLogger.WriteWarning("Cleanup process timed-out.");
            }

            if (!String.IsNullOrEmpty(TestCaseLogDir))
            {
                Directory.CreateDirectory(TestCaseLogDir);
                StreamWriter sw = new StreamWriter(TestCaseLogDir + "\\output.xml");
                XmlSerializer serialiser = new XmlSerializer(typeof(List<Variable>));
                serialiser.Serialize(sw, ContextValues);
                sw.Close();
            }
            GetReport();

            return TestCaseResult;
        }

        private void TranslateResult(bool result, SharedActionBase actionObj, TestStep step)
        {
            if (result)
            {
                if (actionObj.ActionResult == Result.Failed)
                {
                    step.Result = Result.Failed;
                    this.TestCaseResult = Result.Failed;
                    step.Details.AppendLine($"This action returns non-blocking error: {step.Action} please check details in Information.");
                }
                else if (actionObj.ActionResult == Result.CriticalError)
                {
                    step.Result = Result.CriticalError;
                    this.TestCaseResult = Result.Failed;
                    step.Details.AppendLine($"This action returns Fatal Error: {step.Action}");
                    SetIgnoreAllStepsFlag();
                }
                else if (actionObj.ActionResult == Result.Ignored)
                {
                    step.Result = Result.Ignored;
                    step.Details.AppendLine("This action has been ignored by intention.");
                }
                else
                {
                    if (actionObj.ActionResult == Result.Passed)
                    {
                        step.Result = Result.Passed;
                    }
                    step.Details.AppendLine("his checkpoint of the action " + step.Action + " passed");
                }
            }
            else
            {
                step.Result = Result.CriticalError;
                this.TestCaseResult = Result.Failed;
                step.Details.AppendLine("\nThe checkpoint of the action " + step.Action + " did not pass");
                SetIgnoreAllStepsFlag();

            }
        }

        private static void SetIgnoreAllStepsFlag()
        {
            IgnoreAllSteps = true;
        }

        /// <summary>
        /// Initialize the context of the test case
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// cleanup the context of the test case
        /// </summary>
        /// <returns>Detailed information about the clean up</returns>
        public abstract string Cleanup();

    }
}
