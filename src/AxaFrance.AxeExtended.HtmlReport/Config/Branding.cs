using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport.Config
{
    public class Branding
    {
        [JsonProperty("brand")]
        public string Brand { get; set; } = "Brand Name";

        [JsonProperty("application")]
        public string Application { get; set; } = "Application Name";
    }
}
