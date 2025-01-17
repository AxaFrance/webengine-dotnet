using Deque.AxeCore.Commons;

namespace AxaFrance.AxeExtended.HtmlReport
{

    /// <summary>
    /// Enhanced version of AxeResultNode. It contains the screenshot of the node.
    /// </summary>
    public class AxeResultEnhancedNode
    {

        /// <summary>
        /// Initialize the AxeResultEnhancedNode with the AxeResultNode.
        /// </summary>
        /// <param name="node"></param>
        public AxeResultEnhancedNode(AxeResultNode node)
        {
            Node = node;
        }

        /// <summary>
        /// Original Axe result node
        /// </summary>
        public AxeResultNode Node { get; internal set; }


        /// <summary>
        /// The screenshot of the node, if available.
        /// in byte array format, representing the image in PNG format.
        /// </summary>
        public byte[] Screenshot { get; internal set; }
    }
}
