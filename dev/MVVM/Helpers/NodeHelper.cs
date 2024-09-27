using Calc.MVVM.Models;

namespace Calc.MVVM.Helpers
{
    public class NodeHelper
    {
        public static void HideAllLabelColor(NodeModel forestItem)
        {    
            forestItem.LabelColorVisible = false;

            foreach (NodeModel nodeItem in forestItem.SubNodeItems)
            {
                HideNodeLabelColor(nodeItem);
            }
        }

        private static void HideNodeLabelColor(NodeModel nodeItem)
        {
            nodeItem.LabelColorVisible = false;

            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                HideNodeLabelColor(subNodeItem);
            }
        }
        public static void ShowSubLabelColor(NodeModel nodeItem)
        {
            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                subNodeItem.LabelColorVisible = true;
            }
        }

        public static void ShowAllSubLabelColor(NodeModel nodeItem)
        {
            nodeItem.LabelColorVisible = true;
            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                ShowAllSubLabelColor(subNodeItem);
            }
        }
    }
}
