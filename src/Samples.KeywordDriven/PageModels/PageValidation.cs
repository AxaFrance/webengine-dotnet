using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.PageModels
{
    public class PageValidation : PageModel
    {
        public WebElementDescription AcceptAgreement = new WebElementDescription
        {
            Id = "agreement-yes"
        };

        public WebElementDescription ValideButton = new WebElementDescription
        {
            TagName = "button",
            InnerText = "Validate the contact"
        };

        public WebElementDescription PrintContract = new WebElementDescription
        {
            TagName = "button",
            ClassName = "btn-dark",
        };

        public WebElementDescription Done = new WebElementDescription
        {
            TagName = "button",
            ClassName = "btn-danger",
        };

        public WebElementDescription LogoutTitle = new WebElementDescription
        {
            TagName = "a",
            InnerText = "Logout",
        };


        public PageValidation(WebDriver driver) : base(driver)
        {
        }
    }
}
