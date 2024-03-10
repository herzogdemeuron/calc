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
    public partial class BuilderView : Window
    {
        private readonly BuilderViewModel BuilderVM;

        public BuilderView(BuilderViewModel bvm)
        {
            BuilderVM = bvm;
            this.DataContext = BuilderVM;
            InitializeComponent();
            MediatorToView.Register("ViewDeselectTreeView", _=>DeselectTreeView());
            MediatorToView.Register("ViewDeselectBrokenNodesTreeView", _ => DeselectBrokenNodesTreeView());
            MediatorToView.Register("ViewSelectBrokenNodesTreeView", node => SelectNodeTreeView((NodeModel)node));
        }

        private void SelectClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleRemove();
        }
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is NodeModel selectedNode)
            {
                BuilderVM.HandleNodeItemSelectionChanged(selectedNode);
                TreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
            }
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



        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await BuilderVM.HandleWindowLoadedAsync();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BuilderVM.HandleWindowClosing();
        }


        private async void NewMappingConfirmed(object sender, RoutedEventArgs e)
        {
            string newName = this.NewNameText.Text.Trim();
            Mapping selectedMapping = this.MappingListBox.SelectedItem as Mapping;
            await BuilderVM.HandleNewMappingCreateAsync(selectedMapping, newName);
        }

        private void NewMappingCanceld(object sender, RoutedEventArgs e)
        {
            this.NewNameText.Text = "";
            MappingListBox.SelectedItem = null;
            BuilderVM.HandleNewMappingCanceled();
        }

        private void MappingErrorClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleMappingErrorClicked();
        }


        
        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            BuilderVM.HandleSideClicked();
        }


        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleRemove();
        }


        private void UpdateRevitClicked(object sender, RoutedEventArgs e)
        {
            var forest = ForestsComboBox.SelectedItem;
            BuilderVM.HandleUpdateRevitClicked(forest as Forest);
        }

 

        private void SaveResultsClicked(object sender, RoutedEventArgs e)
        {
            NewResultNameTextBox.Text = "";
            BuilderVM.SavingVM.HandleSavingResults();
        }

        private async void SaveResultOKClicked( object sender, RoutedEventArgs e)
        {
            await BuilderVM.HandleSendingResults(NewResultNameTextBox.Text);            
        }

        private void SaveResultCancelClicked (object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleCancelSavingResults();
        }

        private void MessageOKClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleMessageClose();
        }

        private async void UpdateMappingClicked(object sender, RoutedEventArgs e)
        {
            await BuilderVM.HandleUpdateMapping();            
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
