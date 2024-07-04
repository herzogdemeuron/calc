using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Helpers;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using Calc.MVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calc.MVVM.Views
{
    public partial class MainView : Window
    {
        private readonly MainViewModel MainVM;

        public MainView(MainViewModel mvm)
        {
            MainVM = mvm;
            this.DataContext = MainVM;
            InitializeComponent();
            MediatorToView.Register("ViewDeselectTreeView", _=>DeselectTreeView());
            MediatorToView.Register("ViewDeselectBrokenNodesTreeView", _ => DeselectBrokenNodesTreeView());
            MediatorToView.Register("ViewSelectBrokenNodesTreeView", node => SelectNodeTreeView((NodeModel)node));
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            MainVM.HandleWindowLoaded();
        }

        private void DeselectTreeView()
        {
            if (TreeView.SelectedItem != null)
            {
                if (TreeView.Tag is TreeViewItem selectedTreeViewItem)
                {
                    selectedTreeViewItem.IsSelected = false;
                }
            }
        }

        private void DeselectBrokenNodesTreeView()
        {
            if (BrokenNodesTreeView.SelectedItem != null)
            {
                if (BrokenNodesTreeView.Tag is TreeViewItem selectedTreeViewItem)
                {
                    selectedTreeViewItem.IsSelected = false;
                }
            }
        }

        private void SelectNodeTreeView(NodeModel node)
        {
            if (node != null)
            {
                //find treeviewitem by node
                var treeViewItem = ViewHelper.FindTreeViewItem(BrokenNodesTreeView, node);
                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = true;
                }
            }
            else
            {
                DeselectBrokenNodesTreeView();
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainVM.HandleWindowClosing();
        }


        private void ForestSelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var forest = ForestsComboBox.SelectedItem;
            MainVM.HandleForestSelectionChanged(forest as Forest);
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainVM.HandleMappingSelectionChanged(MappingsComboBox.SelectedItem as Mapping);
        }

        private void MappingListViewSelected(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).ContainerFromElement(e.OriginalSource as DependencyObject) as ListViewItem;
            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                e.Handled = true;
            }
        }

        private void NewMappingClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleNewMappingClicked();
        }

        private async void NewMappingConfirmed(object sender, RoutedEventArgs e)
        {
            string newName = this.NewNameText.Text.Trim();
            Mapping selectedMapping = this.MappingListView.SelectedItem as Mapping;
            await MainVM.HandleNewMappingCreateAsync(selectedMapping, newName);
        }

        private void NewMappingCanceld(object sender, RoutedEventArgs e)
        {
            this.NewNameText.Text = "";
            MappingListView.SelectedItem = null;
            MainVM.HandleNewMappingCanceled();
        }

        private void MappingErrorClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleMappingErrorClicked();
        }


        private void BrokenNodeSelected(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeModel selectedNode)
            {
                MainVM.HandleBrokenNodeSelectionChanged(selectedNode);
                BrokenNodesTreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
            }
        }
        private void HandleIgnoreSelectedBrokenNode(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeModel selectedNode)
            {
                MainVM.HandleIgnoreSelectedBrokenNode(selectedNode);
            }
        }

        private void HandleIgnoreAllBrokenNodes(object sender, RoutedEventArgs e)
        {
            MainVM.HandleIgnoreAllBrokenNodes();
        }

        private void ErrorMappingSideClickDown(object sender, MouseButtonEventArgs e)
        {
            MainVM.HandleErrorMappingSideClicked();
        }

        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is NodeModel selectedNode)
            {
                MainVM.HandleNodeItemSelectionChanged(selectedNode);
                TreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
            }
        }

        private void SetBuildupClicked(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            SetBuildupWithTag(tag);
        }

        // set the main or sub buildup calling the buildup selection view
        // decide which buildup to set based on the tag
        private void SetBuildupWithTag(string tag)
        {
            bool setMain = tag.Contains("Main");

            MainVM.HandleSelectingBuildup(setMain);

            var buildupSelectionView = new BuildupSelectionView(MainVM.BuildupSelectionVM);

            var result = buildupSelectionView.ShowDialog();
            if (result == true)
            {
                MainVM.HandleBuildupSelected(setMain);
            }
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            MainVM.HandleSideClicked();
        }

        private void InheritClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleInherit();
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleRemove();
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
            MainVM.HandleUpdateRevitClicked(forest as Forest);
        }

        private void SaveResultsClicked(object sender, RoutedEventArgs e)
        {
            NewResultNameTextBox.Text = "";
            MainVM.SavingVM.HandleSavingResults();
        }

        private async void SaveResultOKClicked( object sender, RoutedEventArgs e)
        {
            await MainVM.HandleSendingResults(NewResultNameTextBox.Text);            
        }

        private void SaveResultCancelClicked (object sender, RoutedEventArgs e)
        {
            MainVM.HandleCancelSavingResults();
        }

        private void MessageOKClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleMessageClose();
        }

        private async void UpdateMappingClicked(object sender, RoutedEventArgs e)
        {
            await MainVM.HandleUpdateMapping();            
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
