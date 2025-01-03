using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;

namespace Samples.KeywordDriven.Actions
{
    public class Login : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        // Runs the action to fill username, password and lick on login button.
        public override void DoAction()
        {
            Browser.Navigate().GoToUrl(GetParameter("URL_RECETTE"));
            PageModels.PageLogin login = new PageModels.PageLogin(Browser);
            login.UserName.SetValue(GetParameter("User"));
            login.UserName.SetSecure(GetParameter("EncPassword"));
            login.ButtonLogin.Click();
        }

        // Verifies if this action goes well.
        public override bool DoCheckpoint()
        {
            PageModels.PageLogin login = new PageModels.PageLogin(Browser);
            if (login.ErrorMessage.Exists(5) && !string.IsNullOrWhiteSpace(login.ErrorMessage.InnerText))
            {
                Information.AppendLine("Error message is shown, login failed");
                return false;
            }
            return true;
        }
    }
}
