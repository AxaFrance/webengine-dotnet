using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Samples.KeywordDriven.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.KeywordDriven.TestCases
{
    public class UpdateCustomerInfo : TestCaseWeb
    {
        public UpdateCustomerInfo() {
            TestSteps = [
                new TestStep{ Action = nameof(Login)},
                new TestStep{ Action = nameof(SearchCustomer)},
                new TestStep{ Action = nameof(UpdateCustomer)},
                new TestStep{ Action = nameof(ValidateInformation)},
                new TestStep{ Action = nameof(Logout)},
            ];
        }
    }
}
