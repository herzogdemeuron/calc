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

        public NodeModel CurrentForestItem { get; set; } = new NodeModel(null, null);
        public NodeModel CurrentDarkForestItem { get; set; } = new NodeModel(null, null);

        private List<NodeModel> ForestSelectionRecord = new List<NodeModel>(); // TO BE REVISED: record which forest were selected, then to reset the color, for performance
        public ObservableCollection<NodeModel> NodeSource
        { get => new ObservableCollection<NodeModel> { CurrentForestItem, CurrentDarkForestItem }; }

        public NodeTreeViewModel(CalcStore calcStore, IVisualizer visualizer)
        {
            Store = calcStore;
            this.visualizer = visualizer;
            BranchesSwitch = false;
        }

        /// <summary>
        /// Update the source of forest node and dark forest node
        /// </summary>
        public void UpdateNodeSource()
        {
            CurrentForestItem = new NodeModel(Store.ForestSelected, this);
            CurrentDarkForestItem = new NodeModel(Store.DarkForestSelected, this);
            OnPropertyChanged(nameof(NodeSource));
            ReColorAllNodes(true);
            DeselectNodes();
        }

        /// <summary>
        /// re-map all nodes in the current forest
        /// return the broken forest
        /// </summary>
        public Forest ReMapAllNodes()
        {
            if (Store.ForestSelected == null) return null;
            if (Store.MappingSelected == null) return null;

            var brokenForest = MappingHelper.ApplyMappingToForestItem(CurrentForestItem, Store, Store.MappingSelected); // this should be passed to mapping error?
            //ReColorAllNodes();

            return brokenForest;

        }

        /// <summary>
        /// reset all node label colors property according to the current branch/assembly switch
        /// report to the visualizer
        /// </summary>
        public void ReColorAllNodes(bool forceRecolorAll = false)
        {
            if (Store.ForestSelected == null) return;
            if (BranchesSwitch == true)
            {
                if (forceRecolorAll)
                {
                    Store.ForestSelected.SetBranchColorsBy("branches");
                    visualizer.IsolateAndColorSubbranchElements(SelectedNodeItem?.Host);
                }
            }
            else
            {
                Store.ForestSelected.SetBranchColorsBy("assemblies");
                visualizer.IsolateAndColorBottomBranchElements(SelectedNodeItem?.Host);
            }

            CurrentForestItem.NotifyNodePropertyChange();
            CurrentDarkForestItem.NotifyNodePropertyChange();

        }

        public void HandleNodeItemSelectionChanged(NodeModel nodeItem)
        {
            if (nodeItem == null) return;
            if (nodeItem.Host == null) return;
            if (CurrentForestItem == null) return;
            SelectedNodeItem = nodeItem;
            NodeHelper.HideAllLabelColor(CurrentForestItem);
            NodeHelper.HideAllLabelColor(CurrentDarkForestItem);

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

            CurrentForestItem.NotifyNodePropertyChange();
            CurrentDarkForestItem.NotifyNodePropertyChange();

            // add the forest to the record
            if (selectedNodeItem.IsDark)
            {
                ForestSelectionRecord.Add(CurrentDarkForestItem);
            }
            else
            {
                ForestSelectionRecord.Add(CurrentForestItem);
            }

        }

        public void ColorNodesToAssembly()
        {
            BranchesSwitch = false;
            if (CurrentForestItem == null) return;
            Store.ForestSelected.SetBranchColorsBy("assemblies");
            CurrentForestItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void ColorNodesToBranch()
        {
            BranchesSwitch = true;
            if (CurrentForestItem == null) return;
            Store.ForestSelected.SetBranchColorsBy("branches");
            CurrentForestItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void DeselectNodes()
        {
            if (CurrentForestItem == null) return;
            NodeHelper.HideAllLabelColor(CurrentForestItem);
            NodeHelper.HideAllLabelColor(CurrentDarkForestItem);

            SelectedNodeItem = null;
            CurrentForestItem.NotifyNodePropertyChange(); //better ways to do this?
            CurrentDarkForestItem.NotifyNodePropertyChange();

            // take the unique forest from the record
            //var forestItems = ForestSelectionRecord.Distinct().ToList();
            //var resetItems = forestItems.SelectMany(f => f.SubNodeItems).Select(n => n.Host).ToList();
            var resetItems = CurrentForestItem.SubNodeItems.Select(n => n.Host).ToList();
            visualizer.ResetView(resetItems);
            //ForestSelectionRecord.Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
