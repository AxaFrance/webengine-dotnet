using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    /// <summary>
    /// The provider to get additional tags for a rule.
    /// </summary>
    public abstract class AdditionalTagsProvider
    {
        /// <summary>
        /// return a list of additional tags for a rule. if the rule is not found, return an empty list.
        /// </summary>
        /// <param name="ruleId">the identifier of the rule</param>
        /// <returns>a list of additional tags for a given rule.</returns>
        public abstract IEnumerable<string> GetTagsByRule(string ruleId);
    }
}
