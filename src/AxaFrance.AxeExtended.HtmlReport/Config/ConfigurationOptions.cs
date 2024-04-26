using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Axe.Extended.HtmlReport.Config
{
    /// <summary>
    /// Options object;
    /// </summary>
    public class ConfigurationOptions
    {
        /// <summary>
        /// mixed(optional) Used to set the branding of the helpUrls
        /// </summary>
        [DataMember(Name = "branding")]
        [JsonProperty("branding")]
        public Branding Branding { get; set; }

        /// <summary>
        /// Used to set the output format that the axe.run function will pass to the callback function. Can pass a reporter name or a custom reporter function. Valid names are:
        /// v1 to use the previous version's format: axe.configure({ reporter: "v1" });
        /// v2 to use the current version's format: axe.configure({ reporter: "v2" });
        /// raw to return the raw result data without formating: axe.configure({ reporter: "raw" });
        /// raw-env to return the raw result data with environment data: axe.configure({ reporter: "raw-env" });
        /// no - passes to return only violation results: axe.configure({ reporter: "no-passes" });
        /// </summary>
        public string reporter { get; set; }

        /// <summary>
        /// Used to add checks to the list of checks used by rules, or to override the properties of existing checks
        /// The checks attribute is an array of check objects
        /// Each check object can contain the following attributes
        /// </summary>
        public Check[] Checks { get; set; }

        /// <summary>
        /// Used to add rules to the existing set of rules, or to override the properties of existing rules
        /// The rules attribute is an Array of rule objects
        /// each rule object can contain the following attributes
        /// </summary>
        public Rule[] Rules{get;set;}
    }
}
