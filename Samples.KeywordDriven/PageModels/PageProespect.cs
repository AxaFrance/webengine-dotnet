using AXA.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.PageModels
{
    public class PageProespect : PageModel
    {

        public WebElementDescription CustomerId = new WebElementDescription
        {
            TagName = "input",
            Name = "prospectId"
        };

        public WebElementDescription CustomerName = new WebElementDescription
        {
            TagName = "input",
            Name = "prospectName"
        };

        public WebElementDescription SearchButton = new WebElementDescription
        {
            TagName = "button",
            InnerText = "Search",
        };

        public WebElementDescription NextStep = new WebElementDescription
        {
            TagName = "button",
            InnerText = "Next Step"
        };

        public PageProespect(WebDriver driver) : base(driver)
        {

        }
    }
}
