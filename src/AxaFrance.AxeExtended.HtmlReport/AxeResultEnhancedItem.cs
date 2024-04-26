using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public class AxeResultEnhancedItem
    {

        public AxeResultEnhancedItem(AxeResultItem item, AxeResultEnhancedNode[] nodes)
        {
            Item = item;
            Nodes = nodes;
        }

        public AxeResultItem Item { get; internal set; }
        public AxeResultEnhancedNode[] Nodes { get; internal set; }
    }
}
