// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.VirtualAuth;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebEngine.Test.UnitTests
{
    /// <summary>
    /// Test class for Passkey authentication using WebAuthn.
    /// Tests passkey registration and login scenarios using virtual authenticator.
    /// Uses CTAP2 protocol with INTERNAL transport for biometric simulation.
    /// Tests against https://www.passkeys.io/ using Hanko authentication component.
    /// Each test uses its own WebDriver instance with independent virtual authenticator.
    /// </summary>
    [TestClass]
    public class Passkey
    {
        private static string CreateTestEmail()
        {
            return $"test.passkey.{DateTime.UtcNow:yyyyMMddHHmmssfff}.{Guid.NewGuid():N}@example.com";
        }

        /// <summary>
        /// Adds a virtual authenticator using CTAP2 protocol and INTERNAL transport
        /// This simulates built-in biometric authentication (like Windows Hello or Touch ID)
        /// </summary>
        private static string AddVirtualAuthenticator(WebDriver driver)
        {
            try
            {
                VirtualAuthenticatorOptions options = new VirtualAuthenticatorOptions()
                    .SetProtocol(VirtualAuthenticatorOptions.Protocol.CTAP2)
                    .SetTransport(VirtualAuthenticatorOptions.Transport.INTERNAL)
                    .SetHasResidentKey(true)        // Support discoverable credentials (passwordless)
                    .SetHasUserVerification(true)   // Simulate biometric capability
                    .SetIsUserVerified(true);       // Auto-verify user (biometric success)

                return driver.AddVirtualAuthenticator(options);
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLine($"Error adding virtual authenticator: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets all credentials from the virtual authenticator.
        /// </summary>
        private static List<Credential> GetCredentials(WebDriver driver)
        {
            if (!(driver is IHasVirtualAuthenticator hasVirtualAuth))
            {
                throw new InvalidOperationException("The WebDriver does not support virtual authenticators.");
            }

            var credentials = hasVirtualAuth.GetCredentials().ToList();
            DebugLogger.WriteLine($"Stored {credentials.Count} credential(s) from virtual authenticator");

            foreach (var cred in credentials)
            {
                DebugLogger.WriteLine($"  - Credential ID: {BitConverter.ToString(cred.Id).Replace("-", "")}");
                DebugLogger.WriteLine($"    RP ID: {cred.RpId}");
                DebugLogger.WriteLine($"    Is Resident: {cred.IsResidentCredential}");
            }

            return credentials;
        }

        private static void WaitForCredentials(WebDriver driver)
        {
            if (!(driver is IHasVirtualAuthenticator hasVirtualAuth))
            {
                throw new InvalidOperationException("The WebDriver does not support virtual authenticators.");
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(_ => hasVirtualAuth.GetCredentials().Any());
        }

        /// <summary>
        /// Restores credentials to the virtual authenticator.
        /// </summary>
        private static void RestoreCredentials(WebDriver driver, IEnumerable<Credential> credentials)
        {
            if (!(driver is IHasVirtualAuthenticator hasVirtualAuth))
            {
                throw new InvalidOperationException("The WebDriver does not support virtual authenticators.");
            }

            var credentialList = credentials.ToList();
            foreach (var credential in credentialList)
            {
                hasVirtualAuth.AddCredential(credential);
            }

            DebugLogger.WriteLine($"Restored {credentialList.Count} credential(s) to virtual authenticator");
        }

        private static List<Credential> RegisterPasskey()
        {
            WebDriver driver = null;

            try
            {
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);
                AddVirtualAuthenticator(driver);
                var pageModel = new PasskeyPageModel(driver);
                var testEmail = CreateTestEmail();

                DebugLogger.WriteLine($"Test email for registration: {testEmail}");
                driver.Navigate().GoToUrl("https://www.passkeys.io/");
                pageModel.CreateAccountLink.Click();
                pageModel.CreateAccountEmailInput.SetValue(testEmail);
                pageModel.CreateAccountContinueButton.Click();
                pageModel.CreatePasskeyButton.Click();
                WaitForCredentials(driver);

                return GetCredentials(driver);
            }
            finally
            {
                try
                {
                    driver?.Quit();
                }
                catch { }
                try
                {
                    driver?.Close();
                }
                catch { }
                try
                {
                    driver?.Dispose();
                }
                catch { }
            }
        }

        [TestMethod]
        [TestCategory("Passkey")]
        [Priority(1)]
        public void Test01_RegisterPasskey()
        {
            var credentials = RegisterPasskey();

            Assert.IsTrue(credentials.Count > 0, "At least one credential should be created during registration");
        }

        [TestMethod]
        [TestCategory("Passkey")]
        [Priority(2)]
        public void Test02_LoginWithPasskey()
        {
            WebDriver driver = null;

            try
            {
                // Arrange
                var credentials = RegisterPasskey();
                Assert.IsTrue(credentials.Count > 0, "There should be at least one credential to use for login");
                DebugLogger.WriteLine($"Starting login test with {credentials.Count} stored credential(s)");

                // Create a new WebDriver instance with virtual authenticator
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
                AddVirtualAuthenticator(driver);

                RestoreCredentials(driver, credentials);

                var pageModel = new PasskeyPageModel(driver);

                // Navigate to login page
                driver.Navigate().GoToUrl("https://www.passkeys.io/");

                // Conditional mediation may complete the login before the button is available.
                var authenticationStateWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                authenticationStateWait.Until(_ => pageModel.IsOnSignInPage() || pageModel.IsOnProfilePage());

                // Act - Click "Sign in with a passkey" button when conditional mediation did not log in.
                if (pageModel.IsOnSignInPage())
                {
                    DebugLogger.WriteLine("Clicking 'Sign in with a passkey' button");
                    pageModel.SignInWithPasskeyButton.Click();
                    authenticationStateWait.Until(_ => pageModel.IsOnProfilePage());
                }

                Assert.IsTrue(pageModel.IsOnProfilePage(), "The passkey login should display the profile page");
            }
            finally
            {
                // Cleanup: Dispose the driver after test
                try
                {
                    driver?.Quit();
                }
                catch { }
                try
                {
                    driver?.Close();
                }
                catch { }
                try
                {
                    driver?.Dispose();
                }
                catch { }
            }
        }
    }
}
