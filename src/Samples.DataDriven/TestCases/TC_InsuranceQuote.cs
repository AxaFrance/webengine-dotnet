using AxaFrance.WebEngine.Web;
using Samples.DataDriven.Actions;

namespace Samples.DataDriven.TestCases
{
    //Test case: Insurance Quote
    public class TC_InsuranceQuote : TestCaseWeb
    {
        public TC_InsuranceQuote() {
            
            //Define Test steps 
            TestSteps = new AxaFrance.WebEngine.TestStep[] {
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Login)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(SearchProspect)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Underwriting)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(ChooseOfferOptions)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(ValidateContract)},
                new AxaFrance.WebEngine.TestStep{ Action = nameof(Logout)},
            };
        }
    }
}
