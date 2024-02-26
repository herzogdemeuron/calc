using Calc.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Calc.MVVM.Helpers
{
    public static class ViewHelper
    {
        public static TreeViewItem FindTreeViewItem(ItemsControl itemsControl, NodeViewModel node)
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
    }
}
