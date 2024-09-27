using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calc.MVVM.Views
{
    public partial class MaterialSelectionView : Window
    {
        private double originalLeft; // used to store the original left position of the window, for window ajustment when expander is expanded or collapsed

        private readonly MaterialSelectionViewModel MaterialSelectionVM;
        public Material SelectedMaterial { get => MaterialSelectionVM.SelectedMaterial; }

        public MaterialSelectionView(MaterialSelectionViewModel materialSelectionVM)
        {
            this.MaterialSelectionVM = materialSelectionVM;
            this.DataContext = MaterialSelectionVM;
            InitializeComponent();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            MaterialSelectionVM.HandleSearchTextChanged(currentText);
        }

        private void ListViewSelected(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).ContainerFromElement(e.OriginalSource as DependencyObject) as ListViewItem;
            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                e.Handled = true;
            }
        }

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            if(listView.SelectedItem!=null)
            {
                listView.ScrollIntoView(listView.SelectedItem);
            }
        }

        private void ListViewItemMaterialDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OKClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Enables scrolling item by item in the list view
        /// </summary>
        private void ListViewPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = ViewHelper.FindVisualChild<ScrollViewer>(sender as ListView);

            if (e.Delta > 0)
            {
                viewer.LineUp();
            }
            if (e.Delta < 0)
            {
                viewer.LineDown();
            }
            e.Handled = true;
        }

        private void SourceCheckChanged(object sender, RoutedEventArgs e)
        {
            MaterialSelectionVM.HandleSourceCheckChanged();
        }

        private void WindowLocationChanged(object sender, RoutedEventArgs e)
        {
            originalLeft = this.Left;
        }

        private void WindowLocationChanged(object sender, EventArgs e)
        {
            originalLeft = this.Left;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            if (expander != null)
            {
                this.Width += 220;
                this.Left = originalLeft; // Keep the left position fixed
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            if (expander != null)
            {
                this.Width -= 220;
                this.Left = originalLeft; // Keep the left position fixed
            }
        }
    }
}
