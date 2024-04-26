using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public static class ExtensionMethods
    {
        public static string GetImpact(this AxeResultItem item)
        {
            
            var impact = item.Impact;
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.Impact;
            }
            if(impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.All.FirstOrDefault()?.Impact;
            }
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.Any.FirstOrDefault()?.Impact;
            }
            if (impact == null)
            {
                impact = item.Nodes.FirstOrDefault()?.None.FirstOrDefault()?.Impact;
            }
            return impact;
        }

    }
}
