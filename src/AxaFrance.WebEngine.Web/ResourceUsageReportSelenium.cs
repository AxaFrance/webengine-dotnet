using OpenQA.Selenium;
using OpenQA.Selenium.BiDi;
using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// represents a report that measures the resource usage of the web site during a user journey.
    /// </summary>
    public class ResourceUsageReportSelenium : ResourceUsageReport
    {

        private INetwork network;

        private ResourceUsageReportSelenium(WebDriver driver)
        {
            network = driver.Manage().Network;
            network.NetworkRequestSent += NetworkRequestSent;
            network.NetworkResponseReceived += NetworkRequestReceived;
            network.StartMonitoring();
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        /// <summary>
        /// Start monitoring the web traffic.
        /// </summary>
        /// <param name="driver">Selenium WebDriver instance</param>
        /// <returns></returns>
        public static ResourceUsageReport StartMonitoring(WebDriver driver)
        {
            return new ResourceUsageReportSelenium(driver);
        }

        /// <summary>
        /// Stop minotoring web traffics. Run it before you need to close the test session or write report to file.
        /// </summary>
        public override void StopMonitoring()
        {
            network.StopMonitoring();
            stopwatch?.Stop();
        }

        Stopwatch stopwatch = null;

        private void NetworkRequestSent(object sender, NetworkRequestSentEventArgs e)
        {
            if (stopwatch is null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            var request = new NetworkRequest { RequestId = e.RequestId, Url = e.RequestUrl, Method = e.RequestMethod };
            //calcualte the size of request by adding request headers and body
            request.Request = e.RequestHeaders.Sum(x => x.Key.Length + x.Value.Length) + (e.RequestPostData?.Length ?? 0);
            lock (NetworkRequests)
            {
                NetworkRequests[request.RequestId] = request;
            }
            request.Sent = DateTime.Now;

            request.TimeStamp = stopwatch.ElapsedMilliseconds;
        }

        private void NetworkRequestReceived(object sender, NetworkResponseReceivedEventArgs e)
        {
            NetworkRequest request = null;
            lock (NetworkRequests)
            {
                if (NetworkRequests.ContainsKey(e.RequestId))
                {
                    request = NetworkRequests[e.RequestId];
                    request.Received = DateTime.Now;
                }
                else
                {
                    request = new NetworkRequest { RequestId = e.RequestId, Url = e.ResponseUrl };
                    request.Sent = request.Received = DateTime.Now;
                    NetworkRequests[request.RequestId] = request;
                }
                //calcualte the size of response by adding response headers and body
                request.IsCached = IsCached(NetworkRequests, e);
                request.Reponse = e.ResponseHeaders.Sum(x => x.Key.Length + x.Value.Length) + (e.ResponseBody?.Length ?? 0);
                request.StatusCode = e.ResponseStatusCode;
                request.ResourceType = e.ResponseResourceType;
            }
        }

        bool IsCached(Dictionary<string, NetworkRequest> requestLogs, NetworkResponseReceivedEventArgs e)
        {
            var hasRequestOfSameUrl = requestLogs.Values.Any(x => x.Url == e.ResponseUrl && x.Reponse != 0);
            return hasRequestOfSameUrl;
        }

    }
}
