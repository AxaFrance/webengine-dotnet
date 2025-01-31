using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class SearchProspect : SharedActionWeb
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageProespect(Browser);
            var customerId = GetParameter(ParameterList.CustomerId);
            //Search customer by Id or Name according to external data
            if (string.IsNullOrEmpty(customerId))
            {
                model.CustomerName.SetValue("marie dupont");
                model.SearchButton.Click();
                customerId = model.CustomerId.Value;
                ContextValues.AddItem("CustomerId", customerId);
            }
            else
            {
                model.CustomerId.SetValue(customerId);
            }
            RunAccessibilityTest("Prospect");
            model.NextStep.Click();
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}