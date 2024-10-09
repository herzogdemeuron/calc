using Calc.MVVM.Models;

namespace Calc.MVVM.Helpers
{
    internal class NodeHelper
    {
        internal static void HideAllLabelColor(NodeModel nodeItem)
        {
            nodeItem.LabelColorVisible = false;

            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                HideAllLabelColor(subNodeItem);
            }
        }

        internal static void ShowSubLabelColor(NodeModel nodeItem)
        {
            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                subNodeItem.LabelColorVisible = true;
            }
        }

        internal static void ShowAllSubLabelColor(NodeModel nodeItem)
        {
            nodeItem.LabelColorVisible = true;
            foreach (NodeModel subNodeItem in nodeItem.SubNodeItems)
            {
                ShowAllSubLabelColor(subNodeItem);
            }
        }
    }
}
