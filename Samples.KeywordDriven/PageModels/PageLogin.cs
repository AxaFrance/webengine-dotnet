using AXA.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.PageModels
{
    public class PageLogin : PageModel
    {
        public WebElementDescription UserName = new WebElementDescription
        {
            TagName = "input",
            Name = "login"
        };

        public WebElementDescription Password = new WebElementDescription
        {
            TagName = "input",
            Name = "password"
        };

        public WebElementDescription ButtonLogin = new WebElementDescription
        {
            TagName = "button",
            InnerText = "Login"
        };

        public WebElementDescription ErrorMessage = new WebElementDescription
        {
            TagName = "div",
            ClassName = "alert-danger"
        };

        public PageLogin(WebDriver driver) : base(driver)
        {
        }
    }
}
