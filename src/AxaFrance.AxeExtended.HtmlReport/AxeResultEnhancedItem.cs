using Deque.AxeCore.Commons;

namespace AxaFrance.AxeExtended.HtmlReport
{
    /// <summary>
    /// The enhanced version of AxeResultItem. It contains the result of all nodes that are related to the item.
    /// which contains additional information such as screenshot, bounding box, etc.
    /// </summary>
    public class AxeResultEnhancedItem
    {
        /// <summary>
        /// Initialize the AxeResultEnhancedItem with the AxeResultItem and the related nodes.
        /// </summary>
        /// <param name="item">Original Axe result</param>
        /// <param name="nodes">Enhanced AxeResultNode with screenshot</param>
        public AxeResultEnhancedItem(AxeResultItem item, AxeResultEnhancedNode[] nodes)
        {
            Item = item;
            Nodes = nodes;
        }

        /// <summary>
        /// Original Axe result item
        /// </summary>
        public AxeResultItem Item { get; internal set; }

        /// <summary>
        /// Enhanced AxeResultNode with screenshot
        /// </summary>
        public AxeResultEnhancedNode[] Nodes { get; internal set; }
    }
}
