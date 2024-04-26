using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport.Config
{
    /// <summary>
    /// Branding information for the report
    /// </summary>
    public class Branding
    {
        /// <summary>
        /// Brand name
        /// </summary>
        [JsonProperty("brand")]
        public string Brand { get; set; } = "Brand Name";

        /// <summary>
        /// Application name
        /// </summary>
        [JsonProperty("application")]
        public string Application { get; set; } = "Application Name";
    }
}
