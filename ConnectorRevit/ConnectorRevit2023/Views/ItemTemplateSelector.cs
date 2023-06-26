using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit.Views
{
    public class ItemTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null)
            {
                if (item is Tree)
                {
                    return element.FindResource("TreeItemTemplate") as DataTemplate;
                }
                if (item is Branch)
                {
                    return element.FindResource("BranchItemTemplate") as DataTemplate;
                }

            }
                return null;
        }
    }
}
