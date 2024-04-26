using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport.Config
{
    public class Rule
    {
        /// <summary>
        /// string(required). This uniquely identifies the rule. If the rule already exists, it will be overridden with any of the attributes supplied. 
        /// The attributes below that are marked required, are only required for new rules.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }


        /// <summary>
        /// string(required). Sets the impact of that rule's results
        /// </summary>
        [JsonProperty("impact")]
        public string Impact { get; set; }

        /// <summary>
        /// boolean(option, default false). Override the result of a rule to return "Needs Review" rather than "Violation" if the rule fails.
        /// </summary>
        [JsonProperty("reviewOnFail")]
        public bool ReviewOnFail { get; set; }

        /// <summary>
        /// string(optional, default *). A CSS selector used to identify the elements that are passed into the rule for evaluation.
        /// </summary>
        [JsonProperty("selector ")]
        public string Selector { get; set; } = "*";

        /// <summary>
        /// boolean(optional, default true). This indicates whether elements that are hidden from all users are to be passed into the rule for evaluation.
        /// </summary>
        [JsonProperty("excludeHidden")]
        public bool ExcludeHidden { get; set; } = true;

        /// <summary>
        /// boolean(optional, default true). Whether the rule is turned on. This is a common attribute for overriding.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// boolean(optional, default false). When set to true, this rule is only applied when the entire page is tested.
        /// </summary>
        [JsonProperty("pageLevel")]
        public bool PageLevel { get; set; } = false;

        /// <summary>
        /// array(optional, default []). This is a list of checks that, if none "pass", will generate a violation
        /// </summary>
        [JsonProperty]
        public Check[] Any { get; set; } = new Check[0];

        /// <summary>
        /// array(optional, default []). This is a list of checks that, if any "fails", will generate a violation.
        /// </summary>
        public Check[] All { get; set; } = new Check[0];

        /// <summary>
        /// array(optional, default []). This is a list of checks that, if any "pass", will generate a violation.
        /// </summary>
        public Check[] None { get; set; } = new Check[0];
    }
}
