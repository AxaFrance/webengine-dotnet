using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.AxeExtended.HtmlReport
{
    internal class WcagTagsProvider : TagsProvider
    {
        public override IEnumerable<string> GetTagsByRule(AxeResultItem rule)
        {
            foreach (var tag in rule.Tags)
            {
                if(tag.StartsWith("wcag"))
                {
                    yield return tag;
                }
            }
        }
    }
}
