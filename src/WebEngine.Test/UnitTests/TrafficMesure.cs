using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    public class TrafficMesure
    {
        static WebDriver driver = null;

        static Stopwatch ws = new Stopwatch();


        [ClassCleanup]
        public static void Cleanup()
        {
            networkIntercepter?.StopMonitoring();
            jsEngine?.StopEventMonitoring();
            Thread.Sleep(10000);
            long totalRequestSize = 0;
            long totalResponseSize = 0;
            Dictionary<string, long> sizePerType = new Dictionary<string, long>();
            Dictionary<string, int> countPerType = new Dictionary<string, int>();            
            lock (networkTraffics)
            {
                Console.WriteLine($"Id;Uri;RequestSize;ResponseSize;ResponseTime;ResourceType;StatusCode");
                foreach (var traffic in networkTraffics.Values)
                {
                    long requestSize = traffic.Request.RequestHeaders.Sum(x => x.Key.Length + x.Value.Length) + (traffic.Request.RequestPostData?.Length ?? 0);
                    long responseSize = traffic.Response.ResponseHeaders.Sum(x => x.Key.Length + x.Value.Length) + (traffic.Response?.ResponseBody?.Length ?? 0);
                    Console.WriteLine($"{traffic.Request.RequestId};\"{traffic.Request.RequestUrl}\";{requestSize};{responseSize};{traffic.ResponseTime};{traffic.Response?.ResponseResourceType};{traffic.Response?.ResponseStatusCode}");
                    totalRequestSize += requestSize;
                    totalResponseSize += responseSize;
                    if (sizePerType.ContainsKey(traffic.Response.ResponseResourceType))
                    {
                        sizePerType[traffic.Response.ResponseResourceType] += responseSize;
                    }
                    else
                    {
                        sizePerType[traffic.Response.ResponseResourceType] = responseSize;
                    }
                    if (countPerType.ContainsKey(traffic.Response.ResponseResourceType))
                    {
                        countPerType[traffic.Response.ResponseResourceType]++;
                    }
                    else
                    {
                        countPerType[traffic.Response.ResponseResourceType] = 1;
                    }                
                }

                Console.WriteLine("============ TOTAL PER TYPE =============");
                foreach (var type in sizePerType.Keys)
                {
                    Console.WriteLine($"Type: {type}, Size: {sizePerType[type]}, Count: {countPerType[type]}");
                }
                Console.WriteLine("============ TOTAL =============");
                Console.WriteLine($"Total Request Size: {totalRequestSize}");
                Console.WriteLine($"Total Response Size: {totalResponseSize}");
                Console.WriteLine($"Total Network I/O: {totalRequestSize + totalResponseSize}");

                Console.WriteLine("============ CONSOLE MESSAGES =============");
                foreach (var msg in jsConsoleMessages)
                {
                    Console.WriteLine($"{msg.MessageType}: {msg.MessageContent}");
                }

                Console.WriteLine("============ JAVASCRIPT EXCEPTIONS =============");
                foreach (var ex in jsExceptions)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
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
        static INetwork networkIntercepter;
        static Dictionary<string, NetworkTraffic> networkTraffics = new Dictionary<string, NetworkTraffic>();
        static List<JavaScriptConsoleApiCalledEventArgs> jsConsoleMessages = new List<JavaScriptConsoleApiCalledEventArgs>();
        static List<JavaScriptExceptionThrownEventArgs> jsExceptions = new List<JavaScriptExceptionThrownEventArgs>();
        static IJavaScriptEngine jsEngine;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            if (driver == null)
            {
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
            }
            ws.Start();
            jsEngine = new JavaScriptEngine(driver);
            jsEngine.JavaScriptExceptionThrown += (_, e) =>
            {
                jsExceptions.Add(e);
            };
            jsEngine.JavaScriptConsoleApiCalled += (_, e) =>
            {
                jsConsoleMessages.Add(e);
            };
            jsEngine.StartEventMonitoring().ConfigureAwait(false);
            networkIntercepter = driver.Manage().Network;
            networkIntercepter.NetworkRequestSent += (_, e) =>
            {
                lock (networkTraffics)
                {
                    if (!e.RequestUrl.StartsWith("http")) return; //ignore non http requests such as chrome extension or data urls
                    networkTraffics[e.RequestId] = new NetworkTraffic
                    {
                        RequestId = e.RequestId,
                        Request = e,
                        RequestSentAt = ws.ElapsedMilliseconds,
                    };
                    
                }
            };
            networkIntercepter.NetworkResponseReceived += (_, e) =>
            {
                lock (networkTraffics)
                {
                    if (!e.ResponseUrl.StartsWith("http")) return; //ignore non http requests such as chrome extension or data urls
                    if (networkTraffics.ContainsKey(e.RequestId))
                    {
                        networkTraffics[e.RequestId].Response = e;
                        networkTraffics[e.RequestId].RespnseReceivedAt = ws.ElapsedMilliseconds;
                    }
                    else
                    {
                        Console.WriteLine($"Response received without request: {e.RequestId}");
                    }
                }
            };
            networkIntercepter.StartMonitoring().ConfigureAwait(false);
        }

        [TestMethod]
        public void UserJourney()
        {
            driver.Navigate().GoToUrl("https://www.axa.fr");
            try
            {
                driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
            }
            catch
            {
                //if the cookie button does not exist, it's not a problem
            }
            //scroll down
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            driver.Navigate().GoToUrl("https://www.axa.fr/pro.html");
            try
            {
                driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
            }
            catch
            {
                //if the cookie button does not exist, it's not a problem
            }
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            Thread.Sleep(5000);
            driver.Navigate().GoToUrl("https://www.axa.fr/pro/services-assistance.html");
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            Thread.Sleep(5000);
            driver.Navigate().GoToUrl("https://www.axa.fr/assurance-habitation/demarches-inondation.html");
            Thread.Sleep(5000);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            driver.Navigate().GoToUrl("https://www.axa.fr/pro/devis-assurance-professionnelle.html");
            Thread.Sleep(5000);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            driver.Navigate().GoToUrl("https://www.axa.fr/compte-bancaire.html");
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            Thread.Sleep(5000);
        }

    }

    

    public class NetworkTraffic
    {
        public string RequestId { get; set; }
        public NetworkRequestSentEventArgs Request { get; set; }
        public NetworkResponseReceivedEventArgs Response { get; set; }
        public long RequestSentAt { get; internal set; }
        public long RespnseReceivedAt { get; internal set; }

        public TimeSpan ResponseTime => TimeSpan.FromMilliseconds(RespnseReceivedAt - RequestSentAt);
    }

}
