using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class ValidateContract : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageValidation(Browser);
            model.AcceptAgreement.Click();
            model.ValideButton.Click();

            if (GetParameter(ParameterList.DigitalSign) == "yes")
            {
                model.PrintContract.Click();
                string alertText = Browser.SwitchTo().Alert().Text;
                if (!alertText.Contains("contract has been printed."))
                {
                    this.ActionResult = Result.Failed;
                    this.Information.AppendLine("Message for success printing failed.");
                }
                Browser.SwitchTo().Alert().Accept();
            }
            model.Done.Click();
        }

        public override bool DoCheckpoint()
        {
            var model = new PageModels.PageValidation(Browser);
            return model.LogoutTitle.Exists();
        }
    }
}