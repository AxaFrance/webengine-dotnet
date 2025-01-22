using Deque.AxeCore.Commons;
using System.Collections.Generic;

namespace AxaFrance.AxeExtended.HtmlReport
{
    internal class WcagTagsProvider : TagsProvider
    {
        public override IEnumerable<string> GetTagsByRule(AxeResultItem rule)
        {
            foreach (var tag in rule.Tags)
            {
                if (tag.StartsWith("wcag"))
                {
                    yield return tag;
                }
            }
        }
    }
}
