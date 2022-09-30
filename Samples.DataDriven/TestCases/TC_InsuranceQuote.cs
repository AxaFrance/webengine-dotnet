using AXA.WebEngine.Web;
using Samples.DataDriven.Actions;

namespace Samples.DataDriven.TestCases
{
    public class TC_InsuranceQuote : TestCaseWeb
    {
        public TC_InsuranceQuote() {
            TestSteps = new AXA.WebEngine.TestStep[] {
                new AXA.WebEngine.TestStep{ Action = nameof(Login)},
                new AXA.WebEngine.TestStep{ Action = nameof(SearchProspect)},
                new AXA.WebEngine.TestStep{ Action = nameof(Underwriting)},
                new AXA.WebEngine.TestStep{ Action = nameof(ChooseOfferOptions)},
                new AXA.WebEngine.TestStep{ Action = nameof(ValidateContract)},
                new AXA.WebEngine.TestStep{ Action = nameof(Logout)},
            };
        }
    }
}
