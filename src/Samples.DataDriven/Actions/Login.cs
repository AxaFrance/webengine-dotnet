using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;

namespace Samples.DataDriven.Actions
{
    public class Login : SharedActionWeb
    {
        public override Variable[] RequiredParameters => null;

        // Runs the action to fill username, password and lick on login button.
        public override void DoAction()
        {
            var env = GetParameter(ParameterList.Environment);
            var url = EnvironmentVariables.Current.GetValue($"URL_{env}");
            Browser.Navigate().GoToUrl(url);
            PageModels.PageLogin login = new PageModels.PageLogin(Browser);
            login.UserName.SetValue(GetParameter(ParameterList.Username));
            login.Password.SetValue(GetParameter(ParameterList.Password));
            Screenshot();
            RunAccessibilityTest("Login");
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
