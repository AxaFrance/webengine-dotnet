// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

namespace WebEngine.Test.UnitTests
{
    /// <summary>
    /// Page Model for Passkeys.io authentication pages
    /// Handles Sign In, Create Account, and Passkey Creation flows
    /// </summary>
    public class PasskeyPageModel : PageModel
    {
        public PasskeyPageModel(WebDriver driver) : base(driver)
        {
        }

        #region Sign In Page Elements

        /// <summary>
        /// Email input field on Sign In page
        /// </summary>
        public WebElementDescription SignInEmailInput { get; set; } = new WebElementDescription()
        {
            TagName = "input",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "email"),
                new HtmlAttribute("placeholder", "Email"),
                new HtmlAttribute("autocomplete", "username webauthn")
            }
        };

        /// <summary>
        /// Continue button on Sign In page (primary button)
        /// </summary>
        public WebElementDescription SignInContinueButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "submit")
            },
            ClassName = "hanko_button hanko_primary",
            InnerText = "Continue"
        };

        /// <summary>
        /// "Sign in with a passkey" button (secondary button)
        /// </summary>
        public WebElementDescription SignInWithPasskeyButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "submit")
            },
            ClassName = "hanko_button hanko_secondary",
            InnerText = "Sign in with a passkey"
        };

        /// <summary>
        /// "Create account" link on Sign In page
        /// </summary>
        public WebElementDescription CreateAccountLink { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            ClassName = "hanko_link",
            InnerText = "Create account"
        };

        #endregion

        #region Create Account Page Elements

        /// <summary>
        /// Email input field on Create Account page
        /// </summary>
        public WebElementDescription CreateAccountEmailInput { get; set; } = new WebElementDescription()
        {
            TagName = "input",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "email"),
                new HtmlAttribute("placeholder", "Email"),
                new HtmlAttribute("autocomplete", "email")
            }
        };

        /// <summary>
        /// Continue button on Create Account page
        /// </summary>
        public WebElementDescription CreateAccountContinueButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "submit")
            },
            ClassName = "hanko_button hanko_primary",
            InnerText = "Continue"
        };

        /// <summary>
        /// "Sign in" link on Create Account page
        /// </summary>
        public WebElementDescription SignInLink { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            ClassName = "hanko_link",
            InnerText = "Sign in"
        };

        /// <summary>
        /// Headline for Create Account page
        /// </summary>
        public WebElementDescription CreateAccountHeadline { get; set; } = new WebElementDescription()
        {
            TagName = "h1",
            ClassName = "hanko_headline hanko_grade1",
            InnerText = "Create account"
        };

        #endregion

        #region Create Passkey Page Elements

        /// <summary>
        /// "Create a passkey" button on passkey creation page
        /// </summary>
        public WebElementDescription CreatePasskeyButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute("type", "submit")
            },
            ClassName = "hanko_button hanko_primary",
            InnerText = "Create a passkey"
        };

        /// <summary>
        /// Back button on passkey creation page
        /// </summary>
        public WebElementDescription PasskeyBackButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            ClassName = "hanko_link",
            InnerText = "Back"
        };

        /// <summary>
        /// Skip button on passkey creation page
        /// </summary>
        public WebElementDescription PasskeySkipButton { get; set; } = new WebElementDescription()
        {
            TagName = "button",
            ClassName = "hanko_link",
            InnerText = "Skip"
        };

        /// <summary>
        /// Headline for Create Passkey page
        /// </summary>
        public WebElementDescription CreatePasskeyHeadline { get; set; } = new WebElementDescription()
        {
            TagName = "h1",
            ClassName = "hanko_headline hanko_grade1",
            InnerText = "Create a passkey"
        };

        #endregion

        #region Common Elements

        /// <summary>
        /// Error box that displays error messages
        /// </summary>
        public WebElementDescription ErrorBox { get; set; } = new WebElementDescription()
        {
            TagName = "section",
            ClassName = "hanko_errorBox"
        };

        /// <summary>
        /// Error message text within error box
        /// </summary>
        public WebElementDescription ErrorMessage { get; set; } = new WebElementDescription()
        {
            Id = "errorMessage"
        };

        /// <summary>
        /// Main content container
        /// </summary>
        public WebElementDescription ContentSection { get; set; } = new WebElementDescription()
        {
            TagName = "section",
            ClassName = "hanko_content"
        };

        /// <summary>
        /// Footer section containing navigation links
        /// </summary>
        public WebElementDescription Footer { get; set; } = new WebElementDescription()
        {
            TagName = "section",
            ClassName = "hanko_footer"
        };

        /// <summary>
        /// Sign In page headline
        /// </summary>
        private WebElementDescription SignInHeadline { get; set; } = new WebElementDescription()
        {
            TagName = "h1",
            ClassName = "hanko_headline hanko_grade1",
            InnerText = "Sign in"
        };

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if we are on the Sign In page
        /// </summary>
        public bool IsOnSignInPage()
        {
            return SignInHeadline.Exists();
        }

        /// <summary>
        /// Checks if we are on the Create Account page
        /// </summary>
        public bool IsOnCreateAccountPage()
        {
            return CreateAccountHeadline.Exists();
        }

        /// <summary>
        /// Checks if we are on the Create Passkey page
        /// </summary>
        public bool IsOnCreatePasskeyPage()
        {
            return CreatePasskeyHeadline.Exists();
        }

        /// <summary>
        /// Navigates to Create Account page from Sign In page
        /// </summary>
        public void GoToCreateAccount()
        {
            if (IsOnSignInPage())
            {
                CreateAccountLink.Click();
            }
        }

        /// <summary>
        /// Navigates to Sign In page from Create Account page
        /// </summary>
        public void GoToSignIn()
        {
            if (IsOnCreateAccountPage())
            {
                SignInLink.Click();
            }
        }

        #endregion
    }
}
