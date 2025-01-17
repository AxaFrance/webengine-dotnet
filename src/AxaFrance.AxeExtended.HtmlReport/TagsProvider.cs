using Deque.AxeCore.Commons;
using System.Collections.Generic;

namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// The provider to get additional tags for a rule.
    /// </summary>
    public abstract class TagsProvider
    {
        /// <summary>
        /// return a list of additional tags for a rule. if the rule is not found, return an empty list.
        /// </summary>
        /// <param name="ruleId">the identifier of the rule</param>
        /// <returns>a list of additional tags for a given rule.</returns>
        public abstract IEnumerable<string> GetTagsByRule(AxeResultItem rule);

        /// <summary>
        /// If additional tags should be shown only on overall report.
        /// The value is determined by tags provider
        /// </summary>
        public bool ShowOnOverallReportOnly { get; internal set; }
    }
}
