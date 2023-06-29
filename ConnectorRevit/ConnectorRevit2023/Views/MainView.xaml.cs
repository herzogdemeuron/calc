using Calc.Core.Objects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            EventMessenger.OnMessageReceived += MessageFromViewModelReceived;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await viewModel.HandleLoadingAsync();
            LoadingOverlay.Visibility  = Visibility.Collapsed;
        }
        private async void ProjectOKClicked(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.SelectedItem == null)
            {
                return;
            }
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

        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is BranchViewModel selectedBranch)
            {
                viewModel.HandleBranchSelectionChanged(selectedBranch);
                TreeView.Tag = e.OriginalSource;
                e.Handled = true;
            }
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.HandleSideClick();
        }

        private void BuildupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.HandleBuildupSelectionChanged();
        }


        private void MessageFromViewModelReceived(string message)
        {
            if (message == "DeselectTreeView")
            {
                if (TreeView.SelectedItem != null)
                {
                    if (TreeView.Tag is TreeViewItem selectedTreeViewItem)
                    {
                        selectedTreeViewItem.IsSelected = false;
                    }
                }
            }
        }

        private void ViewToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            viewModel.HandleViewToggleToBuildup();
        }

        private void ViewToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            viewModel.HandleViewToggleToBranch();
        }

        private void UpdateClicked(object sender, RoutedEventArgs e)
        {
            viewModel.HandleUpdateCalcElements();
        }
        private void CalculateClicked(object sender, RoutedEventArgs e)
        {
            //_viewModel.Calculate();
        }

    }
}
