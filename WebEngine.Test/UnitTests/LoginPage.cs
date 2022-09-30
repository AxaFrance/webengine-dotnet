using AXA.WebEngine.MobileApp;
using AXA.WebEngine.Web;
using OpenQA.Selenium;

namespace WebEngine.Test.UnitTests
{
    public class LoginPage : PageModel
    {
        public WebElementDescription TxtUsername = new WebElementDescription
        {
            TagName = "input",
            Name = "username",
        };

        public WebElementDescription TxtPassword = new WebElementDescription
        {
            TagName = "input",
            Name = "password",
        };

        public WebElementDescription ButtonSubmit = new WebElementDescription
        {
            Id = "submit"
        };

        public WebElementDescription SpanErrorMessage = new WebElementDescription
        {
            TagName = "span",
            ClassName = "alert errormessage"
        };

        public LoginPage(WebDriver driver) : base(driver)
        {
        }
    }


}
