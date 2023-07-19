using Calc.Core.Objects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Calc.ConnectorRevit.ViewModels;
using Calc.ConnectorRevit.Helpers;

namespace Calc.ConnectorRevit.Views
{
    public partial class MainView : Window
    {
        private readonly MainViewModel MainVM;

        public MainView(MainViewModel mvm)
        {
            MainVM = mvm;
            this.DataContext = MainVM;
            InitializeComponent();
            EventMessenger.OnMessageReceived += MessageFromViewModelReceived;
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
            else if (message == "CloseWindow")
            {
                this.Close();
            }
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await MainVM.LoadingVM.HandleLoadingAsync();
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainVM.HandleWindowClosing();
        }

        private async void ProjectOKClicked(object sender, RoutedEventArgs e)
        {
            var project = ProjectsComboBox.SelectedItem;
            if (project == null)
            {
                return;
            }
            LoadingOverlay.Visibility = Visibility.Visible;
            await MainVM.LoadingVM.HandleProjectSelectedAsync(project as Project);
            MainVM.NotifyStoreChange();
            LoadingOverlay.Visibility = Visibility.Collapsed;
            SelectProjectOverlay.Visibility = Visibility.Collapsed;
        }
        
        private void ForestSelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var forest = ForestsComboBox.SelectedItem;
            if (forest == null)
            {
                return;
            }
            MainVM.ForestVM.HandleForestSelectionChanged(forest as Forest);
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var forest = ForestsComboBox.SelectedItem;
            if (forest == null)
            {
                return;
            }
            MainVM.MappingVM.HandleMappingSelectionChanged(MappingsComboBox.SelectedItem as Mapping);
        }

        private void NewMappingClicked(object sender, RoutedEventArgs e)
        {
            MainVM.MappingVM.HandleNewMapping();
            MainVM.NotifyStoreChange();
        }

        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is NodeViewModel selectedBranch)
            {
                MainVM.NodeTreeVM.HandleNodeItemSelectionChanged(selectedBranch);
                TreeView.Tag = e.OriginalSource;
                e.Handled = true;
            }
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            MainVM.NodeTreeVM.DeselectNodes();
        }

        private void BuildupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainVM.BuildupVM.HandleBuildupSelectionChanged();
        }

        private void InheritClicked(object sender, RoutedEventArgs e)
        {
            MainVM.BuildupVM.HandleInherit();
        }

        private void ViewToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleViewToggleToBuildup();
        }

        private void ViewToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleViewToggleToBranch();
        }

        private void UpdateRevitClicked(object sender, RoutedEventArgs e)
        {
            var forest = ForestsComboBox.SelectedItem;
            if (forest == null)
            {
                return;
            }
            MainVM.ForestVM.HandleForestSelectionChanged(forest as Forest);
        }
        private void StartCalcLiveClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleStartCalcLive();
        }

        private void SaveResultsClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleSaveResults();
        }

        private async void UpdateMappingClicked(object sender, RoutedEventArgs e)
        {
            await MainVM.MappingVM.HandleUpdateMapping();
        }
    }
}
