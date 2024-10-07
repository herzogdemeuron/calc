using Calc.MVVM.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calc.MVVM.Helpers
{
    internal static class ViewHelper
    {
        internal static TreeViewItem FindTreeViewItem(ItemsControl itemsControl, NodeModel node)
        {
            if (itemsControl != null && node != null)
            {
                foreach (var item in itemsControl.Items)
                {
                    if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                    {
                        if (treeViewItem.DataContext == node)
                        {
                            return treeViewItem;
                        }
                        else
                        {
                            var foundTreeViewItem = FindTreeViewItem(treeViewItem, node);
                            if (foundTreeViewItem != null)
                            {
                                return foundTreeViewItem;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


    }
}
