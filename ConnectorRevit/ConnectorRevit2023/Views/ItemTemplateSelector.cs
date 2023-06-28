using Calc.Core.Objects;
using System.Windows;
using System.Windows.Controls;

namespace Calc.ConnectorRevit.Views
{
    public class ItemTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null && item is BranchViewModel branchViewModel)
            {
                if (branchViewModel.Branch is Tree)
                {
                    return element.FindResource("TreeItemTemplate") as DataTemplate;
                }
                if (branchViewModel.Branch is Branch)
                {
                    return element.FindResource("BranchItemTemplate") as DataTemplate;
                }

            }
            return null;
        }
    }
}