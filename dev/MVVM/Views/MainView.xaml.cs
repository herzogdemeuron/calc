using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calc.MVVM.Views
{
    public partial class MainView : Window
    {
        private readonly ProjectViewModel MainVM;

        public MainView(ProjectViewModel mvm)
        {
            MainVM = mvm;
            this.DataContext = MainVM;
            InitializeComponent();
            mvm.DeselectTreeView += DeselectTreeView;
            mvm.DeselectBrokenQueryView += DeselectBrokenNodesTreeView;
        }

        private void DeselectTreeView(object sender, EventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                if (TreeView.Tag is TreeViewItem selectedTreeViewItem)
                {
                    selectedTreeViewItem.IsSelected = false;
                }
            }
        }

        private void DeselectBrokenNodesTreeView(object sender, EventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem != null)
            {
                if (BrokenNodesTreeView.Tag is TreeViewItem selectedTreeViewItem)
                {
                    selectedTreeViewItem.IsSelected = false;
                }
            }
        }

        private void SelectNodeTreeView(NodeModel node) // deprecated
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
                //DeselectBrokenNodesTreeView();
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainVM.HandleWindowClosing();
        }

        private void QueryTemplateSelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var qryTemplate = QueryTemplatesComboBox.SelectedItem;
            MainVM.HandleQueryTemplateSelectionChanged(qryTemplate as QueryTemplate);
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainVM.HandleMappingSelectionChanged();
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

        private void SetFirstAssemblyClicked(object sender, RoutedEventArgs e)
        {
            SetAssemblyWithTag(true);
        }

        private void SetSecondAssemblyClicked(object sender, RoutedEventArgs e)
        {
            SetAssemblyWithTag(false);
        }

        // set the main or sub assembly calling the assembly selection view
        private void SetAssemblyWithTag(bool setFirst)
        {
            var canSelect = MainVM.HandleSelectingAssembly(setFirst);

            if (!canSelect) return;

            var assemblySelectionView = new AssemblySelectionView(MainVM.AssemblySelectionVM);
            var result = assemblySelectionView.ShowDialog();

            if (result == true)
            {
                MainVM.HandleAssemblySelected(setFirst);
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

        private void ColorizeClicked(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            switch (tag)
            {
                case "group":
                    this.ColorByAssemblyButton.Opacity = 0.4;
                    this.ColorByAssemblyButton.Uid = "";
                    this.ColorByGroupButton.Opacity = 1.0;
                    this.ColorByGroupButton.Uid = "pack://application:,,,/CalcMVVM;component/Resources/button_color.png";
                    MainVM.HandleViewToggleToBranch();
                    break;
                case "assembly":
                    this.ColorByAssemblyButton.Opacity = 1.0;
                    this.ColorByAssemblyButton.Uid = "pack://application:,,,/CalcMVVM;component/Resources/button_color.png";
                    this.ColorByGroupButton.Opacity = 0.4;
                    this.ColorByGroupButton.Uid = "";
                    MainVM.HandleViewToggleToAssembly();
                    break;
                case "co2":
                    break;
            }
            // DeselectBrokenNodesTreeView(sender, e);
        }

        private void UpdateRevitClicked(object sender, RoutedEventArgs e)
        {
            var qryTemplate = QueryTemplatesComboBox.SelectedItem;
            MainVM.HandleQueryTemplateSelectionChanged(qryTemplate as QueryTemplate, true);
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
