using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Interfaces;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.ViewModels
{
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

        public NodeModel CurrentQueryTemplateItem { get; set; } = new NodeModel(null, null);
        public NodeModel CurrentBlackQuerySetItem { get; set; } = new NodeModel(null, null);
        public ObservableCollection<NodeModel> NodeSource
        { get => new ObservableCollection<NodeModel> { CurrentQueryTemplateItem, CurrentBlackQuerySetItem }; }

        public NodeTreeViewModel(CalcStore calcStore, IVisualizer visualizer)
        {
            Store = calcStore;
            this.visualizer = visualizer;
            BranchesSwitch = false;
        }

        /// <summary>
        /// Update the source of query template node and black query set node
        /// </summary>
        public void UpdateNodeSource()
        {
            CurrentQueryTemplateItem = new NodeModel(Store.QueryTemplateSelected, this);
            CurrentBlackQuerySetItem = new NodeModel(Store.BlackQuerySet, this);
            OnPropertyChanged(nameof(NodeSource));
            ReColorAllNodes(true);
            DeselectNodes();
        }

        /// <summary>
        /// re-map all nodes in the current query template
        /// return the broken query set
        /// </summary>
        public QueryTemplate ReMapAllNodes()
        {
            if (Store.QueryTemplateSelected == null) return null;
            if (Store.MappingSelected == null) return null;

            var brokenQrySet = MappingHelper.ApplyMapping(CurrentQueryTemplateItem, Store, Store.MappingSelected); // this should be passed to mapping error?
            //ReColorAllNodes();

            return brokenQrySet;

        }

        /// <summary>
        /// reset all node label colors property according to the current branch/assembly switch
        /// report to the visualizer
        /// </summary>
        public void ReColorAllNodes(bool forceRecolorAll = false)
        {
            if (Store.QueryTemplateSelected == null) return;
            if (BranchesSwitch == true)
            {
                if (forceRecolorAll)
                {
                    Store.QueryTemplateSelected.SetBranchColorsBy("branches");
                    visualizer.IsolateAndColorSubbranchElements(SelectedNodeItem?.Host);
                }
            }
            else
            {
                Store.QueryTemplateSelected.SetBranchColorsBy("assemblies");
                visualizer.IsolateAndColorBottomBranchElements(SelectedNodeItem?.Host);
            }

            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            CurrentBlackQuerySetItem.NotifyNodePropertyChange();

        }

        public void HandleNodeItemSelectionChanged(NodeModel nodeItem)
        {
            if (nodeItem == null) return;
            if (nodeItem.Host == null) return;
            if (CurrentQueryTemplateItem == null) return;
            SelectedNodeItem = nodeItem;
            NodeHelper.HideAllLabelColor(CurrentQueryTemplateItem);
            NodeHelper.HideAllLabelColor(CurrentBlackQuerySetItem);

            if (BranchesSwitch)
            {
                NodeHelper.ShowSubLabelColor(nodeItem);
                visualizer.IsolateAndColorSubbranchElements(SelectedNodeItem?.Host);
            }
            else
            {
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                visualizer.IsolateAndColorBottomBranchElements(SelectedNodeItem?.Host);
            }

            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            CurrentBlackQuerySetItem.NotifyNodePropertyChange();
        }

        public void ColorNodesToAssembly()
        {
            BranchesSwitch = false;
            if (CurrentQueryTemplateItem == null) return;
            Store.QueryTemplateSelected.SetBranchColorsBy("assemblies");
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void ColorNodesToBranch()
        {
            BranchesSwitch = true;
            if (CurrentQueryTemplateItem == null) return;
            Store.QueryTemplateSelected.SetBranchColorsBy("branches");
            CurrentQueryTemplateItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void DeselectNodes()
        {
            if (CurrentQueryTemplateItem == null) return;
            NodeHelper.HideAllLabelColor(CurrentQueryTemplateItem);
            NodeHelper.HideAllLabelColor(CurrentBlackQuerySetItem);

            SelectedNodeItem = null;
            CurrentQueryTemplateItem.NotifyNodePropertyChange(); //better ways to do this?
            CurrentBlackQuerySetItem.NotifyNodePropertyChange();

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
