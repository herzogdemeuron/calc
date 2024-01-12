﻿using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.ConnectorRevit.Services;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NodeTreeViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        public int MaxBuildups { get; set; } = 2;
        public bool BranchesSwitch { get; set; }
        private NodeViewModel selectedNodeItem;
        public NodeViewModel SelectedNodeItem
        {
            get => selectedNodeItem;
            set
            {
                selectedNodeItem = value;
                OnPropertyChanged(nameof(SelectedNodeItem));
            }
        }

        public NodeViewModel CurrentForestItem { get; set; }
        public ObservableCollection<NodeViewModel> NodeSource 
        { get => new ObservableCollection<NodeViewModel> { CurrentForestItem }; }

        public NodeTreeViewModel(DirectusStore directusStore)
        {
            store = directusStore;
            BranchesSwitch = true;
            MediatorFromVM.Register("ForestSelectionChanged", mapping => UpdateNodeSource((Mapping)mapping));
            MediatorFromVM.Register("MappingSelectionChanged", mapping => RemapAllNodes((Mapping)mapping));
            MediatorFromVM.Register("BuildupSelectionChanged", _ => RecolorAllNodes());
            //MediatorFromVM.Register("BuildupPropertiesSwitched", _ => SelectedNodeItem?.NotifyHostChanged());
            //Mediator.Register("BuildupInherited", _ => RecolorAllNodes());

            MediatorFromVM.Register("MainViewToggleToBuildup", _ => BranchesSwitch = false);
            MediatorFromVM.Register("MainViewToggleToBuildup", _ => ColorNodesToBuildup());

            MediatorFromVM.Register("MainViewToggleToBranch", _ => BranchesSwitch = true);
            MediatorFromVM.Register("MainViewToggleToBranch", _ => ColorNodesToBranch());

            new RevitVisualizer();
            new ResultSender();
            //changing priority: Forest => Mapping => Buildup
        }


        public void UpdateNodeSource(Mapping mapping)
        {
            CurrentForestItem = new NodeViewModel(store, store.ForestSelected, this);
            RemapAllNodes(mapping);
            OnPropertyChanged(nameof(NodeSource));
            RecolorAllNodes(true);
            DeselectNodes();
            //MediatorFromVM.Broadcast("MappingSelectionChanged", mapping);
        }
        public void RemapAllNodes(Mapping mapping)
        {
            if (CurrentForestItem == null) return;
            MappingHelper.ApplyMappingToForestItem(CurrentForestItem, store, mapping, MaxBuildups);
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
                if(forceRecolorAll)
                {
                    store.ForestSelected.SetBranchColorsBy("branches");
                    //MediatorToVisualizer.Broadcast("AllNodesRecoloredOnBranches", SelectedNodeItem); // to visualizer
                }
            }
            else
            {
                store.ForestSelected.SetBranchColorsBy("buildups");
                //MediatorToVisualizer.Broadcast("AllNodesRecoloredOnBuildups", SelectedNodeItem); // to visualizer
            }

            CurrentForestItem.NotifyNodePropertyChange();

        }

        public void HandleNodeItemSelectionChanged(NodeViewModel nodeItem)
        {
            if (nodeItem == null) return;
            if (CurrentForestItem == null) return;
            SelectedNodeItem = nodeItem;
            var host = nodeItem.Host as Branch;
            NodeHelper.HideAllLabelColor(CurrentForestItem);

            if (BranchesSwitch)
            {
                
                NodeHelper.ShowSubLabelColor(nodeItem);
                MediatorToVisualizer.Broadcast("OnBranchItemSelectionChanged", SelectedNodeItem); // to visualizer
            }
            else
            {
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                MediatorToVisualizer.Broadcast("OnBuildupItemSelectionChanged", SelectedNodeItem); // to visualizer
            }

            CurrentForestItem.NotifyNodePropertyChange();
            MediatorFromVM.Broadcast("NodeItemSelectionChanged");
        }

        public void ColorNodesToBuildup()
        {
            if (CurrentForestItem == null) return;
            store.ForestSelected.SetBranchColorsBy("buildups");
            CurrentForestItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void ColorNodesToBranch()
        {
            if (CurrentForestItem == null) return;
            store.ForestSelected.SetBranchColorsBy("branches");
            CurrentForestItem.NotifyNodePropertyChange();
            DeselectNodes();
        }

        public void DeselectNodes()
        {
            if (CurrentForestItem == null) return;
            NodeHelper.HideAllLabelColor(CurrentForestItem);
            MediatorToView.Broadcast("ViewDeselectTreeView"); //send command to the view to deselect treeview
            SelectedNodeItem = null;
            CurrentForestItem.NotifyNodePropertyChange(); //better ways to do this?
            MediatorToVisualizer.Broadcast("TreeViewDeselected", CurrentForestItem); // to the visualizer
            //Mediator.Broadcast("BuildupPropertiesSwitched");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
