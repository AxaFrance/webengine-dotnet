using AXA.WebEngine;
using AXA.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class SearchProspect : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageProespect(Browser);
            model.CustomerName.SetValue("marie dupont");
            model.SearchButton.Click();
            var customerId = model.CustomerId.Value;
            ContextValues.AddItem("CustomerId", customerId);
            model.NextStep.Click();
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}