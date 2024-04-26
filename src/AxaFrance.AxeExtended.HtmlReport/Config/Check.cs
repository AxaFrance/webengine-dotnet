using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport.Config
{
    /// <summary>
    /// Used to add checks to the list of checks used by rules, or to override the properties of existing checks
    /// </summary>
    public class Check
    {
        /// <summary>
        /// string(required). This uniquely identifies the check. If the check already exists, this will result in any supplied check properties being overridden. The properties below that are marked required if new are optional when the check is being overridden.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// string(required for new). The ID of the function that implements the check's functionality. See the metadata-function-map file for all defined IDs.
        /// </summary>
        [JsonProperty("evaluate")]
        public string Evaluate { get; set; }

        /// <summary>
        /// string(optional). The ID of the function that gets called for checks that operate on a page-level basis, to process the results from the iframes.
        /// </summary>
        [JsonProperty("after")]
        public string After { get; set; }

        /// <summary>
        /// mixed(optional). This is the options structure that is passed to the evaluate function and is intended to be used to configure checks. It is the most common property that is intended to be overridden for existing checks.
        /// </summary>
        [JsonProperty("options")]
        public Option[] Options { get; set; }

        /// <summary>
        /// boolean(optional, default true). This is used to indicate whether the check is on or off by default. Checks that are off are not evaluated, even when included in a rule. Overriding this is a common way to disable a particular check across multiple rules.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;
    }
}
