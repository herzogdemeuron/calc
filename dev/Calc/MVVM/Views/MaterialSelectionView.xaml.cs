using Calc.Core.Objects.Materials;
using Calc.MVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calc.MVVM.Views
{
    public partial class MaterialSelectionView : Window
    {
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
    }
}
