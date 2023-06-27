using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Calc.Core.Objects;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Views
{
    public partial class MainView : Window
    {
        private readonly ViewModel viewModel;
        public MainView()
        {
            viewModel = App.ViewModel;
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await viewModel.HandleLoadingAsync();
            LoadingOverlay.Visibility  = Visibility.Collapsed;
        }
        private async void ProjectOKClicked(object sender, RoutedEventArgs e)
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            await viewModel.HandleProjectSelectedAsync(ProjectsComboBox.SelectedItem as Project);
            LoadingOverlay.Visibility = Visibility.Collapsed;
            SelectProjectOverlay.Visibility = Visibility.Collapsed;
        }
        
        private void ForestSelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            viewModel.HandleForestSelectionChanged(ForestsComboBox.SelectedItem as Forest);
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.HandleMappingSelectionChanged(MappingsComboBox.SelectedItem as Mapping);
        }

        private void TreeViewSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            viewModel.HandleBranchSelectionChanged(e.NewValue as BranchViewModel);
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.HandleSideClick();
        }



        private void CalculateClicked(object sender, RoutedEventArgs e)
        {
            //_viewModel.Calculate();
        }
    }
}
