using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    public class PageLogin : PageModel
    {       
        public WebElementDescription TxtUsername = new ()
        {
            TagName = "input",
            Name = "username",
        };

        public WebElementDescription TxtPassword = new()
        {
            TagName = "input",
            Name = "password"
        };

        public WebElementDescription ButtonSubmit = new()
        {
            Id = "submit"
        };

        public WebElementDescription SpanErrorMessage = new() 
        {
            TagName = "span",
            ClassName = "alert errormessage"
        };

        public PageLogin(WebDriver driver) : base(driver) { }
    }
}
