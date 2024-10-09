using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using Calc.MVVM.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Calc.MVVM.Views
{
    public partial class CalcProjectView : Window
    {
        private readonly ProjectViewModel ProjectVM;

        public CalcProjectView(ProjectViewModel pvm)
        {
            ProjectVM = pvm;
            this.DataContext = ProjectVM;
            InitializeComponent();
            pvm.DeselectTreeView += DeselectTreeView;
            pvm.DeselectBrokenQueryView += DeselectBrokenNodesTreeView;
        }

        /// <summary>
        /// Deselects the selected treeview item in the Tag property.
        /// </summary>
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

        /// <summary>
        /// Deselects the selected treeview item in the Tag property.
        /// </summary>
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

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProjectVM.HandleWindowClosing();
        }

        private void QueryTemplateSelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var qryTemplate = QueryTemplatesComboBox.SelectedItem;
            ProjectVM.HandleQueryTemplateSelectionChanged(qryTemplate as QueryTemplate);
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProjectVM.HandleMappingSelectionChanged();
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
            ProjectVM.HandleNewMappingClicked();
        }

        private async void NewMappingConfirmed(object sender, RoutedEventArgs e)
        {
            string newName = this.NewNameText.Text.Trim();
            Mapping selectedMapping = this.MappingListView.SelectedItem as Mapping;
            await ProjectVM.HandleNewMappingCreateAsync(selectedMapping, newName);
        }

        private void NewMappingCanceld(object sender, RoutedEventArgs e)
        {
            this.NewNameText.Text = "";
            MappingListView.SelectedItem = null;
            ProjectVM.HandleNewMappingCanceled();
        }

        private void MappingErrorClicked(object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleMappingErrorClicked();
        }

        /// <summary>
        /// Stores the selected treeview item in the Tag property,
        /// in order to be able to deselect it.
        /// </summary>
        private void BrokenNodeSelected(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeModel selectedNode)
            {
                ProjectVM.HandleBrokenNodeSelectionChanged(selectedNode);
                BrokenNodesTreeView.Tag = e.OriginalSource;
                e.Handled = true;
            }
        }
        private void HandleIgnoreSelectedBrokenNode(object sender, RoutedEventArgs e)
        {
            if (BrokenNodesTreeView.SelectedItem is NodeModel selectedNode)
            {
                ProjectVM.HandleIgnoreSelectedBrokenNode(selectedNode);
            }
        }
        private void HandleIgnoreAllBrokenNodes(object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleIgnoreAllBrokenNodes();
        }

        private void ErrorMappingSideClickDown(object sender, MouseButtonEventArgs e)
        {
            ProjectVM.HandleErrorMappingSideClicked();
        }

        /// <summary>
        /// Stores the selected treeview item in the Tag property,
        /// in order to be able to deselect it.
        /// </summary>
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is NodeModel selectedNode)
            {
                ProjectVM.HandleNodeItemSelectionChanged(selectedNode);
                TreeView.Tag = e.OriginalSource;
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

        /// <summary>
        /// Calls the assembly selection view.
        /// Sets the main or sub assembly with argument setFirst.
        /// </summary>
        private void SetAssemblyWithTag(bool setFirst)
        {
            var canSelect = ProjectVM.HandleSelectingAssembly(setFirst);

            if (!canSelect) return;
            var assemblySelectionView = new AssemblySelectionView(ProjectVM.AssemblySelectionVM);
            var result = assemblySelectionView.ShowDialog();
            if (result == true)
            {
                ProjectVM.HandleAssemblySelected(setFirst);
            }
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            ProjectVM.HandleSideClicked();
        }

        private void InheritClicked(object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleInherit();
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleRemove();
        }

        /// <summary>
        /// Sets the apprearance of the colorize buttons when clicked.
        /// </summary>
        private void ColorizeClicked(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            string image = "pack://application:,,,/CalcMVVM;component/Resources/button_color.png";
            switch (tag)
            {
                case "group":
                    this.ColorByAssemblyButton.Opacity = 0.3;
                    this.ColorByAssemblyButton.Uid = "";
                    this.ColorByGroupButton.Opacity = 1;
                    this.ColorByGroupButton.Uid = image;
                    ProjectVM.HandleViewToggleToBranch();
                    break;
                case "assembly":
                    this.ColorByAssemblyButton.Opacity = 1;
                    this.ColorByAssemblyButton.Uid = image;
                    this.ColorByGroupButton.Opacity = 0.3;
                    this.ColorByGroupButton.Uid = "";
                    ProjectVM.HandleViewToggleToAssembly();
                    break;
                case "co2":
                    break;
            }
        }

        private void UpdateRevitClicked(object sender, RoutedEventArgs e)
        {
            var qryTemplate = QueryTemplatesComboBox.SelectedItem;
            ProjectVM.HandleQueryTemplateSelectionChanged(qryTemplate as QueryTemplate, true);
        }

        private void SaveResultsClicked(object sender, RoutedEventArgs e)
        {
            NewResultNameTextBox.Text = "";
            ProjectVM.SavingVM.HandleSavingResults();
        }

        private async void SaveResultOKClicked( object sender, RoutedEventArgs e)
        {
            await ProjectVM.HandleSendingResults(NewResultNameTextBox.Text);            
        }

        private void SaveResultCancelClicked (object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleCancelSavingResults();
        }

        private void MessageOKClicked(object sender, RoutedEventArgs e)
        {
            ProjectVM.HandleMessageClose();
        }

        private async void UpdateMappingClicked(object sender, RoutedEventArgs e)
        {
            await ProjectVM.HandleUpdateMapping();            
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void OpenHyperLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
