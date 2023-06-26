using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.UI;
using Calc.Core.Objects;
using System.Windows.Controls;


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
            viewModel.SelectedBranch = e.NewValue as Branch;
            viewModel.SetView();
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.ResetView();
        }


        private void CalculateClicked(object sender, RoutedEventArgs e)
        {
            //_viewModel.Calculate();
        }
    }
}
