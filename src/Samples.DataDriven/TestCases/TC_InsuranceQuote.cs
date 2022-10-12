using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Samples.DataDriven.Actions;

namespace Samples.DataDriven.TestCases
{
    //Test case: Insurance Quote
    public class TC_InsuranceQuote : TestCaseWeb
    {
        public TC_InsuranceQuote() {
            
            //Define Test steps 
            TestSteps = new TestStep[] {
                new TestStep{ Action = nameof(Login)},
                new TestStep{ Action = nameof(SearchProspect)},
                new TestStep{ Action = nameof(Underwriting)},
                new TestStep{ Action = nameof(ChooseOfferOptions)},
                new TestStep{ Action = nameof(ValidateContract)},
                new TestStep{ Action = nameof(Logout)},
            };
        }
    }
}
