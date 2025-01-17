using AxaFrance.WebEngine.Web;
using Samples.KeywordDriven.Actions;

namespace Samples.KeywordDriven.TestCases
{
    public class TC_InsuranceQuote : TestCaseWeb
    {
        public TC_InsuranceQuote()
        {
            TestSteps = new AxaFrance.WebEngine.TestStep[] {
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Login)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(SearchProspect)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Underwriting)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(ChooseOfferOptions)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(ValidateContract)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Logout)},
            };
            MeasureResourceUsage = true;
        }
    }
}
