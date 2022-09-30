using AXA.WebEngine;

namespace Samples.KeywordDriven
{
    public class HomeInsuranceTestSuite : TestSuite
    {
        public override KeyValuePair<string, TestCase>[] TestCases => getTestCases();

        private KeyValuePair<string, TestCase>[] getTestCases()
        {
            return new KeyValuePair<string, TestCase>[]
            {
                new KeyValuePair<string, TestCase>("TestCase 1", new TestCases.TC_InsuranceQuote())
            };
        }
    }
}
