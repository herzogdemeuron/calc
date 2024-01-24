using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            MediatorToView.Register("ViewDeselectTreeView", _=>DeselectTreeView());
            MediatorToView.Register("ViewDeselectBrokenNodesTreeView", _ => DeselectBrokenNodesTreeView());
            MediatorToView.Register("ViewSelectBrokenNodesTreeView", node => SelectNodeTreeView((NodeViewModel)node));
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

        private void SelectNodeTreeView(NodeViewModel node)
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



        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await MainVM.HandleWindowLoadedAsync();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainVM.HandleWindowClosing();
        }

        private async void ProjectOKClicked(object sender, RoutedEventArgs e)
        {
            var project = ProjectsComboBox.SelectedItem;
            await MainVM.HandleProjectSelectedAsync(project as Project);
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

        private void NewMappingClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleNewMappingClicked();
        }

        private async void NewMappingConfirmed(object sender, RoutedEventArgs e)
        {
            string newName = this.NewNameText.Text.Trim();
            Mapping selectedMapping = this.MappingListBox.SelectedItem as Mapping;
            await MainVM.HandleNewMappingCreateAsync(selectedMapping, newName);
        }

        private void NewMappingCanceld(object sender, RoutedEventArgs e)
        {
            this.NewNameText.Text = "";
            MappingListBox.SelectedItem = null;
            MainVM.HandleNewMappingCanceled();
        }

        private void MappingErrorClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleMappingErrorClicked();
        }


        private void BrokenNodeSelected(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeViewModel selectedNode)
            {
                MainVM.HandleBrokenNodeSelectionChanged(selectedNode);
                BrokenNodesTreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
            }
        }
        private void HandleIgnoreSelectedBrokenNode(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeViewModel selectedNode)
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
            if (TreeView.SelectedItem is NodeViewModel selectedNode)
            {
                MainVM.HandleNodeItemSelectionChanged(selectedNode);
                TreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
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
        private void StartCalcLiveClicked(object sender, RoutedEventArgs e)
        {
            MainVM.HandleStartCalcLive();
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
