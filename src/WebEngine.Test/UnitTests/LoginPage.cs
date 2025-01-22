using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

namespace WebEngine.Test.UnitTests
{
    public class LoginPage : PageModel
    {
        [FindsBy(How = How.TagName, Using = "input")]
        public WebElementDescription TxtUsername = new WebElementDescription
        {
            Name = "username",
        };

        [FindsBy(How = How.TagName, Using = "input")]
        [FindsBy(How = How.Name, Using = "password")]
        public WebElementDescription TxtPassword = new WebElementDescription();

        [FindsBy(How = How.Id, Using = "submit")]
        public WebElementDescription ButtonSubmit { get; set; } = new WebElementDescription();

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
