using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.PageModels
{
    public class PageOffer : PageModel
    {

        public WebElementDescription OfferEconomic = new WebElementDescription
        {
            Id = "offer1"
        };

        public WebElementDescription OfferBalanced = new WebElementDescription
        {
            Id = "offer2"
        };

        public WebElementDescription OfferOptimized = new WebElementDescription
        {
            Id = "offer3"
        };


        public WebElementDescription PriceBalanced = new WebElementDescription
        {
            TagName = "div",
            ClassName = "form-label alert-warning",
        };

        public WebElementDescription Option24x7Support = new WebElementDescription
        {
            CssSelector = "#ant > table > tbody > tr:nth-child(8) > td.text-center > input[type=checkbox]"
        };

        public WebElementDescription OptionAnnualPayment = new WebElementDescription
        {
            XPath = "//*[@id=\"ant\"]/table/tbody/tr[9]/td[2]/input"
        };

        public WebElementDescription Subscribe = new WebElementDescription
        {
            InnerText = "Subscribe",
            TagName = "button"
        };

        public PageOffer(WebDriver driver) : base(driver)
        {
        }
    }
}
