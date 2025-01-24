using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// represents a report that measures the resource usage of the web site during a user journey.
    /// </summary>
    public class ResourceUsageReport : IDisposable
    {
        /// <summary>
        /// list of all network requests made by the web site during the user journey.
        /// </summary>
        public Dictionary<string, NetworkRequest> NetworkRequests { get; set; } = new Dictionary<string, NetworkRequest>();

        /// <summary>
        /// list of all javascript errors that occurred during the user journey.
        /// </summary>
        public List<JavaScriptError> JavaScriptError { get; set; } = new List<JavaScriptError>();

        /// <summary>
        /// Stop monitoring and dispose the used ressources
        /// </summary>
        public void Dispose()
        {
            StopMonitoring();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stop minotoring web traffics. Run it before you need to close the test session or write report to file.
        /// </summary>
        public virtual void StopMonitoring()
        {

        }

    }
}
