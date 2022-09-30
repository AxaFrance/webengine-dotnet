using AXA.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.PageModels
{
    public class PageUnderWriting : PageModel
    {

        public WebElementDescription StreetNumber = new WebElementDescription
        {
            TagName = "input",
            Name = "streetNumber"
        };

        public WebElementDescription StreetName = new WebElementDescription
        {
            TagName = "input",
            Name = "streetName"
        };

        public WebElementDescription City = new WebElementDescription()
        {
            TagName = "input",
            Name = "city"
        };

        public WebElementDescription PostCode = new WebElementDescription
        {
            TagName = "input",
            Name = "postcode"
        };


        public WebElementDescription Region = new WebElementDescription
        {
            TagName = "input",
            Name = "region"
        };

        public WebElementDescription Country = new WebElementDescription
        {
            TagName = "select",
            Name = "country"
        };

        public WebElementDescription TypeOfHomeRadioGroup = new WebElementDescription
        {
            Name = "homeType"
        };

        public WebElementDescription NumberOfRoomSelect = new WebElementDescription
        {
            Name = "rooms"
        };

        public WebElementDescription HomeSurface = new WebElementDescription
        {
            Name = "surface"
        };

        public WebElementDescription HouseFloors = new WebElementDescription
        {
            Name = "floors"
        };

        public WebElementDescription HouseBackyardSurface = new WebElementDescription
        {
            Name = "surface-backyard"
        };

        public WebElementDescription HouseHasSwimmingPoolRadioGroup = new WebElementDescription
        {
            Name = "pool"
        };

        public WebElementDescription ApptTotalFloors = new WebElementDescription
        {
            Name = "total-floors"
        };

        public WebElementDescription ApptMyFloor = new WebElementDescription
        {
            Name = "my-floors"
        };

        public WebElementDescription ApptHasElevatorRadioGroup = new WebElementDescription
        {
            Name = "elevator"
        };

        public WebElementDescription NextStep = new WebElementDescription
        {
            TagName = "button",
            InnerText = "Next Step"
        };

        public WebElementDescription AntecedentsRadioGroup = new WebElementDescription
        {
            Name = "antecedents"
        };

        public WebElementDescription AntecedentType = new WebElementDescription
        {
            Name = "accident-type"
        };

        public WebElementDescription AntecedentDate = new WebElementDescription
        {
            TagName = "input",
            Name = "date"
        };

        public WebElementDescription AntecedentResponsability = new WebElementDescription
        {
            Name = "responsability"
        };


        public PageUnderWriting(WebDriver driver) : base(driver)
        {
        }
    }
}
