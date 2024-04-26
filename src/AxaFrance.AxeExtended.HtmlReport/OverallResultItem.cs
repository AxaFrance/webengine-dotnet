using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public class OverallResultItem
    {
        /// <summary>
        /// Number of points according to the criticity.
        /// </summary>
        public int RulePoint { get; set; }

        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// Wcag Version for example 2.0, 2.1 or 2.2
        /// </summary>
        public Version WcagVersion { get; set; } = new Version(2, 0);

        /// <summary>
        /// Wcag level of current rule: A, AA or AAA
        /// </summary>
        public string WcagLevel { get; set; } = string.Empty;

        /// <summary>
        /// A list of equivalent WCAG Critirias
        /// </summary>
        public string[] WcagCritirias { get; set; } = Array.Empty<string>();
    }
}
