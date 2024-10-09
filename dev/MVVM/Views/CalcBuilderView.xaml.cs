using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calc.MVVM.Views
{
    public partial class CalcBuilderView : Window
    {
        private readonly BuilderViewModel BuilderVM;
        private Point _lastMouseDown;
        private object _draggedItem;

        public CalcBuilderView(BuilderViewModel bvm)
        {
            BuilderVM = bvm;
            this.DataContext = BuilderVM;
            InitializeComponent();
            bvm.DeselectTreeView += DeselectTreeView;
        }

        /// <summary>
        /// If there are less than 5 items in the treeview, expand all after selection.
        /// </summary>
        private void SelectClicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
            BuilderVM.HandleSelect();
            this.Show();
            if (TreeView.Items?.Count < 5) ExpandAll(this.TreeView, true);
        }

        /// <summary>
        /// Stores the selected treeview item in the Tag property,
        /// in order to be able to deselect it.
        /// </summary>
        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is ICalcComponent selectedCompo)
            {
                BuilderVM.HandleComponentSelectionChanged(selectedCompo);
                TreeView.Tag = e.OriginalSource; // save selected treeviewitem for deselecting
                e.Handled = true;
            }
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

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleWindowLoaded();
        }

        private void AssemblyNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            BuilderVM.HandleAssemblyNameChanged(currentText);
        }

        private void AssemblyNameFocusLost(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAssemblyNameSetFinished();
        }

        private void AssemblyCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            BuilderVM.HandleAssemblyCodeChanged(currentText);
        }

        private void AssemblyCodeFocusLost(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAssemblyCodeSetFinished();
        }

        private void CountAmountClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAmountClicked(Unit.piece);
        }

        private void LengthAmountClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAmountClicked(Unit.m);
        }

        private void AreaAmountClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAmountClicked(Unit.m2);
        }

        private void VolumeAmountClicked(object sender, RoutedEventArgs e)
        {
            BuilderVM.HandleAmountClicked(Unit.m3);
        }

        /// <summary>
        /// Handles the enter keydown event for the ratio textbox.
        /// </summary>
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

        private void SetFirstMaterialClicked(object sender, RoutedEventArgs e)
        {
            SetMaterial(true);
        }

        private void SetSecondMaterialClicked(object sender, RoutedEventArgs e)
        {
            SetMaterial(false);
        }

        /// <summary>
        /// Calls the material selection view.
        /// Sets the main or sub material with argument setFirst.
        /// </summary>
        private void SetMaterial(bool setFirst)
        {
            BuilderVM.HandleSelectingMaterial(setFirst);
            var materialSelectionView = new MaterialSelectionView(BuilderVM.MaterialSelectionVM);

            var result = materialSelectionView.ShowDialog();
            if (result == true)
            {
                BuilderVM.HandleMaterialSelected(setFirst);
            }
        }

        private async void SaveAssemblyClicked(object sender, RoutedEventArgs e)
        {
            await BuilderVM.HandleSaveAssembly();
        }

        /// <summary>
        /// Drags the window.
        /// </summary>
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

        /// <summary>
        /// Previews image snapshot at the mouse position.
        /// </summary>
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

        /// <summary>
        /// Rearranges assembly component in the treeview by drag and drop.
        /// </summary>
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

        /// <summary>
        /// Rearranges assembly component in the treeview by drag and drop.
        /// </summary>
        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        /// <summary>
        /// Rearranges assembly component in the treeview by drag and drop.
        /// </summary>
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
            int oldIndex = BuilderVM.AssemblyCreationVM.AssemblyComponents.IndexOf(droppedData);
            int newIndex = BuilderVM.AssemblyCreationVM.AssemblyComponents.IndexOf(targetData);

            BuilderVM.AssemblyCreationVM.MoveAssemblyComponent(oldIndex, newIndex);
            _draggedItem = null;
        }

        /// <summary>
        /// If the double clicked item is a treeviewitem, call set main material;
        /// if the double clicked item is not a treeviewitem, expand/collapse all
        /// </summary>
        private void TreeViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = FindAncestorOrSelf<TreeViewItem>(e.OriginalSource as DependencyObject);
            if (treeViewItem == null)
            {
                bool shouldExpand = ShouldExpandItems(TreeView);
                ExpandAll(TreeView, shouldExpand);
            }
            else if (!treeViewItem.HasItems)
            {
                SetMaterial(true);
            }
        }

        /// <summary>
        /// Fetches if the items are expanded.
        /// </summary>
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

        /// <summary>
        /// Expands/Collapses all items in the treeview.
        /// </summary>
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
