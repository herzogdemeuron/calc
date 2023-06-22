using System.Windows;
using Autodesk.Revit.UI;


namespace Calc.ConnectorRevit.Views
{
    public partial class MainView : Window
    {
        private readonly ViewModel viewModel;
        public MainView()
        {
            viewModel = App.ViewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.InitializeAsync();

            // Hide the loading overlay
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }

        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            viewModel.SelectedItem = e.NewValue as TreeViewItem;
            viewModel.SetView();
        }


        private void CalculateClicked(object sender, RoutedEventArgs e)
        {
            //_viewModel.Calculate();
        }

        private void ResetViewClicked(object sender, RoutedEventArgs e)
        {
            viewModel.ResetView();
        }
    }
}
