using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class ChooseOfferOptions : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageOffer(Browser);
            var offer = GetParameter(ParameterList.Offer);
            switch (offer)
            {
                case "Economic":
                    model.OfferEconomic.Click();
                    break;
                case "Balanced":
                    model.OfferBalanced.Click();
                    break;
                case "Optimized":
                    model.OfferOptimized.Click();
                    break;
            }

            if (GetParameter(ParameterList.Option24x7) == "yes")
            {
                model.Option24x7Support.Click();
            }

            if (GetParameter(ParameterList.OptionAnnualPayment) == "yes")
            {
                model.OptionAnnualPayment.Click();
            }


            RunAccessibilityTest("Offer");
            this.Screenshot("screenshot for selected offer and option");

        }

        public override bool DoCheckpoint()
        {
            var model = new PageModels.PageOffer(Browser);
            var expectedPrice = GetParameter(ParameterList.VerifyAmountBalanced);
            var actualPrice = model.PriceBalanced.GetText();
            model.Subscribe.Click();
            if (!string.IsNullOrEmpty(expectedPrice))
            {
                ContextValues.AddItem("Expected Price", expectedPrice);
                ContextValues.AddItem("Actual Price", actualPrice);

                if (actualPrice.Contains(expectedPrice))
                {
                    //verification is passed
                    return true;
                }
                else
                {
                    //verification is failed.
                    ActionResult = Result.Failed;
                    Information.AppendLine($"Price verification fail. Expected: {expectedPrice}, Actual: {expectedPrice}");
                    return false;
                }
            }
            // if expected value is not provided, then do not check price.
            return true;
        }
    }
}