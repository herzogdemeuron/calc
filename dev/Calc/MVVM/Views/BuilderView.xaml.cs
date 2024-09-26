using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calc.MVVM.Views
{
    public partial class BuilderView : Window
    {
        private readonly BuilderViewModel BuilderVM;
        private Point _lastMouseDown;
        private object _draggedItem;

        public BuilderView(BuilderViewModel bvm)
        {
            BuilderVM = bvm;
            this.DataContext = BuilderVM;
            InitializeComponent();
            MediatorToView.Register("ViewDeselectTreeView", _=>DeselectTreeView());
        }

        private void SelectClicked(object sender, RoutedEventArgs e)
        {
            // hide the window when selecting elelemts
            this.Hide();
            BuilderVM.HandleSelect();
            // show the window again
            this.Show();
            // if there are less than 5 items in the treeview, expand all
            if (TreeView.Items?.Count < 5) ExpandAll(this.TreeView, true);
        }
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is ICalcComponent selectedCompo)
            {
                BuilderVM.HandleComponentSelectionChanged(selectedCompo);
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

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleWindowLoaded();
        }

        private void BuildupNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            BuilderVM.HandleBuildupNameChanged(currentText);
        }

        private void BuildupNameFocusLost(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleBuildupNameSetFinished();
        }

        private void BuildupCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            BuilderVM.HandleBuildupCodeChanged(currentText);
        }

        private void BuildupCodeFocusLost(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleBuildupCodeSetFinished();
        }


        private void AmountClicked(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            BuilderVM.HandleAmountClicked(tag);
        }

        private void RatioTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // move focus to the next element
                e.Handled = true;
                var element = e.OriginalSource as UIElement;
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void ReduceMaterialClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleReduceMaterial();
        }

        private void SetMaterialClicked(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            SetMaterialWithTag(tag);
        }

        // set the main or sub material calling the material selection view
        // decide which material to set based on the tag
        private void SetMaterialWithTag(string tag)
        {
            bool setMain = tag.Contains("First");

            BuilderVM.HandleSelectingMaterial(setMain);

            var materialSelectionView = new MaterialSelectionView(BuilderVM.MaterialSelectionVM);

            var result = materialSelectionView.ShowDialog();
            if (result == true)
            {
                BuilderVM.HandleMaterialSelected(setMain);
            }
        }


        private void MessageOKClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleMessageClose();
        }

        private async void SaveBuildupClicked(object sender, RoutedEventArgs e)
        {
            await BuilderVM.HandleSaveBuildup();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException)
                {
                    // ignored
                }
            }
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnCaputureClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleCaptureClicked();
        }

        private void CaptureMouseEnter(object sender, MouseEventArgs e)
        {
            BuilderVM.HandleCaptureMouseOver(true);
            ImagePreviewPopup.IsOpen = true;
        }

        private void CaptureMouseLeave(object sender, MouseEventArgs e)
        {
            BuilderVM.HandleCaptureMouseOver(false);
            ImagePreviewPopup.IsOpen = false;
        }

        private void CaptureMouseMove(object sender, MouseEventArgs e)
        {
            if (!ImagePreviewPopup.IsOpen) return;

            ImagePreviewPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
            ImagePreviewPopup.HorizontalOffset = e.GetPosition(this).X;
            ImagePreviewPopup.VerticalOffset = e.GetPosition(this).Y;
        }

        private void SideClickDown(object sender, MouseButtonEventArgs e)
        {
            _lastMouseDown = e.GetPosition(TreeView);
            BuilderVM.HandleSideClicked();
        }
        private void TreeViewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(TreeView);
                if (e.LeftButton == MouseButtonState.Pressed &&
                    (_draggedItem == null || _draggedItem is AssemblyComponent) &&
                    (Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0 ||
                     Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    var item = TreeView.SelectedItem as AssemblyComponent;
                    if (item != null)
                    {
                        _draggedItem = item;
                        DragDrop.DoDragDrop(TreeView, item, DragDropEffects.Move);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void TreeViewDrop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(typeof(AssemblyComponent)) as AssemblyComponent;
            if (droppedData == null) return;

            // Get the drop target
            var targetItem = e.OriginalSource as FrameworkElement;
            if (targetItem == null) return;

            TreeViewItem targetTreeViewItem = FindAncestorOrSelf<TreeViewItem>(targetItem);
            if (targetTreeViewItem == null) return;

            var targetData = targetTreeViewItem.DataContext as AssemblyComponent;
            if (targetData == null || ReferenceEquals(droppedData, targetData)) return;

            // Find indices
            int oldIndex = BuilderVM.BuildupCreationVM.BuildupComponents.IndexOf(droppedData);
            int newIndex = BuilderVM.BuildupCreationVM.BuildupComponents.IndexOf(targetData);

            BuilderVM.BuildupCreationVM.MoveBuildupComponent(oldIndex, newIndex);
            _draggedItem = null;
        }

        private void TreeViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = FindAncestorOrSelf<TreeViewItem>(e.OriginalSource as DependencyObject);
            // if the double clicked item is not a treeviewitem, expand/collapse all
            if (treeViewItem == null)
            {
                bool shouldExpand = ShouldExpandItems(TreeView);
                ExpandAll(TreeView, shouldExpand);
            }
            // if the double clicked item is a treeviewitem, call set main material
            else if (!treeViewItem.HasItems)
            {
                SetMaterialWithTag("Main");
            }
        }

        private bool ShouldExpandItems(ItemsControl itemsControl)
        {
            foreach (var item in itemsControl.Items)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                {
                    if (container.IsExpanded)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Expand/Collapse all items in the treeview
        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)  ExpandAll(childControl, expand);
                
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null) item.IsExpanded = expand;
            }
        }

        private static T FindAncestorOrSelf<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                if (obj is T tObj) return tObj;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

    }
}
