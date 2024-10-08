using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// For the queries tree view in calc project.
    /// </summary>
    public class NodeTreeViewModel : INotifyPropertyChanged
    {
        private NodeModel selectedNodeItem;
        private readonly IVisualizer visualizer;
        public CalcStore Store;
        public bool BranchesSwitch { get; set; }
        public HslColor CurrentColor { get => SelectedNodeItem?.Host?.HslColor ?? ItemPainter.DefaultColor; }
        public NodeModel SelectedNodeItem
        {
            get => selectedNodeItem;
            set
            {
                selectedNodeItem = value;
                OnPropertyChanged(nameof(SelectedNodeItem));
            }
        }
        private Visibility treeViewVisibility;
        public Visibility TreeViewVisibility
        {
            get => treeViewVisibility;
            set
            {
                treeViewVisibility = value;
                OnPropertyChanged(nameof(TreeViewVisibility));
            }
        }
        private Visibility emptyNoteVisibility;
        public Visibility EmptyNoteVisibility
        {
            get => emptyNoteVisibility;
            set
            {
                emptyNoteVisibility = value;
                OnPropertyChanged(nameof(EmptyNoteVisibility));
            }
        }
        public NodeModel CurrentQueryTemplateItem { get; set; } = new NodeModel(null, null);
        public NodeModel CurrentLeftoverQuerySetItem { get; set; } = new NodeModel(null, null);
        public ObservableCollection<NodeModel> NodeSource
        { get => new ObservableCollection<NodeModel> { CurrentQueryTemplateItem, CurrentLeftoverQuerySetItem }; }

        public NodeTreeViewModel(CalcStore calcStore, IVisualizer visualizer)
        {
            Store = calcStore;
            this.visualizer = visualizer;
            BranchesSwitch = false;
            TreeViewVisibility = Visibility.Collapsed;
            EmptyNoteVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Updates the source of query template node and leftover query set node.
        /// </summary>
        internal void UpdateNodeSource()
        {
            CurrentQueryTemplateItem = new NodeModel(Store.QueryTemplateSelected, this);
            CurrentLeftoverQuerySetItem = new NodeModel(Store.LeftoverQuerySet, this);
            TreeViewVisibility = Visibility.Visible;
            EmptyNoteVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(NodeSource));
            ReColorAllNodes(true);
            DeselectNodes();
        }

        /// <summary>
        /// Re-maps all nodes in the current query template.
        /// </summary>
        /// <returns>The broken query set</returns>
        internal QueryTemplate ReMapAllNodes()
        {
            if (Store.QueryTemplateSelected == null) return null;
            if (Store.MappingSelected == null) return null;

            var brokenQrySet = MappingHelper.ApplyMapping(CurrentQueryTemplateItem, Store, Store.MappingSelected); // this should be passed to mapping error?
            //ReColorAllNodes();

            return brokenQrySet;

        }

        /// <summary>
        /// Resets all node label colors property according to the current branch/assembly switch,
        /// feedbacks to the visualizer.
        /// </summary>
        internal void ReColorAllNodes(bool forceRecolorAll = false)
        {
            if (Store.QueryTemplateSelected == null) return;
            if (BranchesSwitch == true)
            {
                if (forceRecolorAll)
                {
                    Store.QueryTemplateSelected.SetBranchColorsBy("branches");
                    visualizer.IsolateAndColorizeSubbranchElements(SelectedNodeItem?.Host);
                }
            }
            else
            {
                Store.QueryTemplateSelected.SetBranchColorsBy("assemblies");
                visualizer.IsolateAndColorizeBottomBranchElements(SelectedNodeItem?.Host);
            }
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            CurrentLeftoverQuerySetItem.NotifyNodePropertyChange(); // todo: check if this is needed
        }

        /// <summary>
        /// Controls lable color visibility with the selected node,
        /// feedbacks to the visualizer.
        /// </summary>
        internal void HandleNodeItemSelectionChanged(NodeModel nodeItem)
        {
            if (nodeItem == null) return;
            if (nodeItem.Host == null) return;
            if (CurrentQueryTemplateItem == null) return;
            SelectedNodeItem = nodeItem;
            NodeHelper.HideAllLabelColor(CurrentQueryTemplateItem);
            NodeHelper.HideAllLabelColor(CurrentLeftoverQuerySetItem); // todo: check if this is needed
            if (BranchesSwitch)
            {
                NodeHelper.ShowSubLabelColor(nodeItem);
                visualizer.IsolateAndColorizeSubbranchElements(SelectedNodeItem?.Host);
            }
            else
            {
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                visualizer.IsolateAndColorizeBottomBranchElements(SelectedNodeItem?.Host);
            }
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            CurrentLeftoverQuerySetItem.NotifyNodePropertyChange(); // todo: check if this is needed
        }

        /// <summary>
        /// Resets all node (branch) colors by assembly assignment.
        /// </summary>
        internal void ColorNodesToAssembly()
        {
            BranchesSwitch = false;
            if (CurrentQueryTemplateItem == null) return;
            Store.QueryTemplateSelected.SetBranchColorsBy("assemblies");
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        /// <summary>
        /// Resets all node (branch) colors by branching.
        /// </summary>
        internal void ColorNodesToBranch()
        {
            BranchesSwitch = true;
            if (CurrentQueryTemplateItem == null) return;
            Store.QueryTemplateSelected.SetBranchColorsBy("branches");
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        /// <summary>
        /// Hide all label colors,
        /// feedbacks to the visualizer.
        /// </summary>
        internal void DeselectNodes()
        {
            if (CurrentQueryTemplateItem == null) return;
            NodeHelper.HideAllLabelColor(CurrentQueryTemplateItem);
            NodeHelper.HideAllLabelColor(CurrentLeftoverQuerySetItem); // todo: check if this is needed
            SelectedNodeItem = null;
            CurrentQueryTemplateItem.NotifyNodePropertyChange(); //better ways to do this?
            CurrentLeftoverQuerySetItem.NotifyNodePropertyChange(); // todo: check if this is needed
            var resetItems = CurrentQueryTemplateItem.SubNodeItems.Select(n => n.Host).ToList();
            visualizer.ResetView(resetItems);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
