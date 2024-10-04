using Calc.MVVM.Models;

namespace Calc.MVVM.Helpers
{
    public class NodeHelper
    {
        internal static void HideAllLabelColor(NodeModel nodeItem)
        {
            nodeItem.LabelColorVisible = false;

            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                HideAllLabelColor(subNodeItem);
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
