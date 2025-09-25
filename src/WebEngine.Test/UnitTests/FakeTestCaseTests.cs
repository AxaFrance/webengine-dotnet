using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Report;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace WebEngine.Test.UnitTests
{
    // Dummy shared action that calls StopTestCase in DoAction
    public class DummyStopAction : SharedActionBase
    {
        public DummyStopAction() : base(Array.Empty<Variable>()) { }
        public override Variable[] RequiredParameters => Array.Empty<Variable>();
        public override void DoAction(object Context)
        {
            ActionReport.Result = Result.Passed;
            StopTestCase();
        }
        public override bool DoCheckpoint(object Context) => true;
        protected override void Screenshot(string name) { }
    }

    // Dummy shared action for a normal step
    public class DummyStepAction : SharedActionBase
    {
        public DummyStepAction() : base(Array.Empty<Variable>()) { }
        public override Variable[] RequiredParameters => Array.Empty<Variable>();
        public override void DoAction(object Context)
        {
            ActionResult = Result.Passed;
        }
        public override bool DoCheckpoint(object Context) => true;
        protected override void Screenshot(string name) { }
    }

    // Fake test case for unit test
    public class FakeTestCase : TestCase
    {
        public FakeTestCase()
        {
            Name = "FakeTestCase";
            TestSteps = new TestStep[]
            {
                new TestStep { Action = nameof(DummyStopAction) },
                new TestStep { Action = nameof(DummyStepAction) },
                new TestStep { Action = nameof(DummyStepAction) }
            };
        }
        public override void Initialize() { }
        public override string Cleanup() => string.Empty;
    }

    [TestClass]
    public class StopTestCaseTests
    {
        [TestMethod]
        public void Test_StopTestCase_StopsAllSteps()
        {
            var testCase = new FakeTestCase();
            var report = new TestCaseReport { ActionReports = new List<ActionReport>() };
            var result = testCase.Run(report);
            // Only the first step should run, others should be ignored
            Assert.AreEqual(Result.None, testCase.TestSteps[0].Result);
            Assert.AreEqual(Result.Ignored, testCase.TestSteps[1].Result);
            Assert.AreEqual(Result.Ignored, testCase.TestSteps[2].Result);

            Assert.AreEqual(Result.Passed, report.ActionReports[0].Result);
            Assert.AreEqual(Result.Ignored, report.ActionReports[1].Result);
            Assert.AreEqual(Result.Ignored, report.ActionReports[2].Result);
        }
    }
}
