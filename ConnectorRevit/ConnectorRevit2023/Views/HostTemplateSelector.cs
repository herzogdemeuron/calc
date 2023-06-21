using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit.Views
{
    public class HostTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object host, DependencyObject container)
        {
            if (container is FrameworkElement element && host != null)
            {
                if (host is Tree)
                {
                    Tree tree = host as Tree;
                    Debug.WriteLine("TreeTemplate");

                    Debug.WriteLine(tree.GetType());
                    Debug.Write(tree.Roots.Count);
                    return element.FindResource("TreeTemplate") as DataTemplate;
                }

                if (host is Branch)
                {
                    Debug.WriteLine("BranchTemplate");
                    return element.FindResource("BranchTemplate") as DataTemplate;
                }

                Debug.WriteLine(host.GetType());
            }
            Debug.WriteLine("NullTemplate");
            return null;
        }
    }
}

