using Deque.AxeCore.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axe.Extended.HtmlReport
{
    public class AxeResultEnhancedNode
    {

        public AxeResultEnhancedNode(AxeResultNode node)
        {
            Node = node;
        }

        public AxeResultNode Node { get; internal set; }
        public byte[] Screenshot { get; internal set; }
    }
}
