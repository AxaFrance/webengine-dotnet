using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// Extension methods for AxeResultItem
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Get the impact of the AxeResultItem.
        /// </summary>
        /// <param name="item"><see cref="AxeResultItem"/> object</param>
        /// <returns>The impact value</returns>
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
