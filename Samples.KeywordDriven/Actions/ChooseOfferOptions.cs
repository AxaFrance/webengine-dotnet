using AXA.WebEngine;
using AXA.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class ChooseOfferOptions : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageOffer(Browser);
            model.OfferBalanced.Click();
            model.Option24x7Support.Click();
            this.Screenshot("screenshot for selected offer and option");
            model.Subscribe.Click();
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}