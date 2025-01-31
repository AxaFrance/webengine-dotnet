using AxaFrance.WebEngine;
using System.Collections.Generic;

namespace Samples.DataDriven
{
    public class HomeInsuranceTestSuite : TestSuite
    {
        public override KeyValuePair<string, TestCase>[] TestCases => getTestCases();

        private KeyValuePair<string, TestCase>[] getTestCases()
        {
            List<KeyValuePair<string, TestCase>> testCases = new List<KeyValuePair<string, TestCase>>();

            // TestSuiteData.Current.TestDataList is provided by framework
            // The test data is provided in XML format and loaded via argument `-data` of WebRunner.
            foreach (var testdata in TestSuiteData.Current.TestDataList)
            {
                var tc = new TestCases.TC_InsuranceQuote()
                {
                    Name = testdata.TestName,
                    AccessibilityReportTitle = testdata.TestName
                };
                testCases.Add(new KeyValuePair<string, TestCase>(testdata.TestName, tc));
            }

            return testCases.ToArray();
        }
    }
}
