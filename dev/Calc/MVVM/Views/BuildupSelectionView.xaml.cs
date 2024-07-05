using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Calc.MVVM.Views
{
    public partial class BuildupSelectionView : Window
    {

        private readonly DispatcherTimer hoverTimer;
        private ListViewItem currentItem;
        private readonly BuildupSelectionViewModel BuildupSelectionVM;

        public BuildupSelectionView(BuildupSelectionViewModel buildupSelectionVM)
        {
            this.BuildupSelectionVM = buildupSelectionVM;
            this.DataContext = BuildupSelectionVM;

            hoverTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5)};
            hoverTimer.Tick += HoverTimerTick;

            InitializeComponent();
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var currentText = (sender as TextBox).Text;
            BuildupSelectionVM.HandleSearchTextChanged(currentText);
        }

        private void ListViewSelected(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).ContainerFromElement(e.OriginalSource as DependencyObject) as ListViewItem;
            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                e.Handled = true;
            }
        }

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            if(listView.SelectedItem!=null)
            {
                listView.ScrollIntoView(listView.SelectedItem);
            }
        }

        private void ListViewItemBuildupDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OKClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Enables scrolling item by item in the list view
        /// </summary>
        private void ListViewPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = ViewHelper.FindVisualChild<ScrollViewer>(sender as ListView);

            if (e.Delta > 0)
            {
                viewer.LineUp();
            }
            if (e.Delta < 0)
            {
                viewer.LineDown();
            }
            e.Handled = true;
        }

        private void SourceCheckChanged(object sender, RoutedEventArgs e)
        {
            BuildupSelectionVM.HandleSourceCheckChanged();
        }


        private void ListViewItemMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item == currentItem) return;
                ImagePreviewPopup.IsOpen = false;
                currentItem = item;
                hoverTimer.Stop();
                hoverTimer.Start();                
            }
        }

        private void ListViewItemMouseLeave(object sender, MouseEventArgs e)
        {
            ImagePreviewPopup.IsOpen = false;
            currentItem = null;
            hoverTimer.Stop();
        }

        private void ListViewItemMouseMove(object sender, MouseEventArgs e)
        {
            if (!ImagePreviewPopup.IsOpen) return;
            ImagePreviewPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
            ImagePreviewPopup.HorizontalOffset = e.GetPosition(this).X;
            ImagePreviewPopup.VerticalOffset = e.GetPosition(this).Y;
        }

        private async void HoverTimerTick(object sender, EventArgs e)
        {
            hoverTimer.Stop();
            if (currentItem != null && IsMouseOverTarget(currentItem))
            {
                ImagePreviewPopup.IsOpen = true;
                var buildup = currentItem.DataContext as Buildup;
                await BuildupSelectionVM.LoadImage(buildup);
            }
        }

        private bool IsMouseOverTarget(UIElement target)
        {
            Point position = Mouse.GetPosition(target);
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(position);
        }


    }
}
