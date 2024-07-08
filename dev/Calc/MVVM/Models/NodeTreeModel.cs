using Calc.MVVM.Helpers;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Services;
using Calc.Core;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Calc.Core.Interfaces;
using System.Linq;
using Calc.Core.Color;

namespace Calc.MVVM.Models
{
    public class NodeTreeModel : INotifyPropertyChanged
    {
        public CalcStore Store;
        public List<Buildup> AllBuildups => Store.BuildupsAll;
        public int MaxBuildups { get; set; } = 2;
        public bool BranchesSwitch { get; set; }

        private IVisualizer visualizer;
        public HslColor CurrentColor { get => SelectedNodeItem?.Host?.HslColor ?? ItemPainter.DefaultColor; }

        private NodeModel selectedNodeItem;
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

        private List<NodeModel> ForestSelectionRecord = new List<NodeModel>(); // record which forest were selected, then to reset the color, for performance
        public ObservableCollection<NodeModel> NodeSource
        { get => new ObservableCollection<NodeModel> { CurrentForestItem, CurrentDarkForestItem }; }

        public NodeTreeModel(CalcStore calcStore, IVisualizer visualizer)
        {
            Store = calcStore;
            this.visualizer = visualizer;
            BranchesSwitch = true;
            MediatorFromVM.Register("ForestSelectionChanged", mapping => UpdateNodeSource((Mapping)mapping));
            MediatorFromVM.Register("MappingSelectionChanged", mapping => RemapAllNodes((Mapping)mapping));
            MediatorFromVM.Register("BuildupSelectionChanged", _ => RecolorAllNodes());

            MediatorFromVM.Register("MainViewToggleToBuildup", _ => BranchesSwitch = false);
            MediatorFromVM.Register("MainViewToggleToBuildup", _ => ColorNodesToBuildup());

            MediatorFromVM.Register("MainViewToggleToBranch", _ => BranchesSwitch = true);
            MediatorFromVM.Register("MainViewToggleToBranch", _ => ColorNodesToBranch());
            //new ResultSender();
            //changing priority: Forest => Mapping => Buildup
        }


        public void UpdateNodeSource(Mapping mapping)
        {
            CurrentForestItem = new NodeModel(Store.ForestSelected, this);
            CurrentDarkForestItem = new NodeModel(Store.DarkForestSelected, this);
            RemapAllNodes(mapping);
            OnPropertyChanged(nameof(NodeSource));
            RecolorAllNodes(true);
            DeselectNodes();
        }
        public void RemapAllNodes(Mapping mapping)
        {
            if (CurrentForestItem == null) return;
            var brokenForest = MappingHelper.ApplyMappingToForestItem(CurrentForestItem, Store, mapping, MaxBuildups);
            MediatorFromVM.Broadcast("BrokenForestChanged", brokenForest);
            RecolorAllNodes();
        }

        /// <summary>
        /// reset all node label colors property according to the current branch/buildup switch
        /// report to the visualizer
        /// </summary>
        private void RecolorAllNodes(bool forceRecolorAll = false)
        {
            if (CurrentForestItem == null) return;
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
                Store.ForestSelected.SetBranchColorsBy("buildups");
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

            MediatorFromVM.Broadcast("NodeItemSelectionChanged");

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

        public void ColorNodesToBuildup()
        {
            if (CurrentForestItem == null) return;
            Store.ForestSelected.SetBranchColorsBy("buildups");
            CurrentForestItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void ColorNodesToBranch()
        {
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

            MediatorToView.Broadcast("ViewDeselectTreeView"); //send command to the view to deselect treeview
            SelectedNodeItem = null;
            CurrentForestItem.NotifyNodePropertyChange(); //better ways to do this?
            CurrentDarkForestItem.NotifyNodePropertyChange();

            MediatorFromVM.Broadcast("NodeItemSelectionChanged");
            // take the unique forest from the record
            var forestItems = ForestSelectionRecord.Distinct().ToList();
            var resetItems = forestItems.SelectMany(f => f.SubNodeItems).Select(n => n.Host).ToList();
            visualizer.ResetView(resetItems);
            ForestSelectionRecord.Clear();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
