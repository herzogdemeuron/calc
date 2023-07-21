using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Revit;
using Calc.ConnectorRevit.Services;
using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NodeTreeViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        private bool BranchesSwitch;
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
            Mediator.Register("ForestSelectionChanged", mapping => UpdateNodeSource((Mapping)mapping));
            Mediator.Register("MappingSelectionChanged", mapping => RemapAllNodes((Mapping)mapping));
            Mediator.Register("BuildupSelectionChanged", _ => RecolorAllNodes());
            Mediator.Register("BuildupInherited", _ => RecolorAllNodes());

            Mediator.Register("ViewToggleToBuildup", _ => BranchesSwitch = false);
            Mediator.Register("ViewToggleToBuildup", _ => ColorNodesToBuildup());

            Mediator.Register("ViewToggleToBranch", _ => BranchesSwitch = true);
            Mediator.Register("ViewToggleToBranch", _ => ColorNodesToBranch());

            new RevitVisualizer();
            new ResultSender();
            //changing priority: Forest => Mapping => Buildup
        }

        public void UpdateNodeSource(Mapping mapping)
        {
            CurrentForestItem = new NodeViewModel(store, store.ForestSelected);
            OnPropertyChanged(nameof(NodeSource));
            DeselectNodes();
            Mediator.Broadcast("MappingSelectionChanged", mapping);
        }
        public void RemapAllNodes(Mapping mapping)
        {
            if (CurrentForestItem == null) return;
            MappingHelper.ApplyMappingToForestItem(CurrentForestItem, store, mapping);
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        private void RecolorAllNodes()
        {
            if (CurrentForestItem == null) return;
            if (BranchesSwitch == true)
            {
                store.ForestSelected.SetBranchColorsBy("branches");
            }
            else
            {
                store.ForestSelected.SetBranchColorsBy("buildups");
            }
            CurrentForestItem.NotifyLabelColorChange();
            Mediator.Broadcast("AllNodesRecolored", SelectedNodeItem); // to visualizer
        }

        public void HandleNodeItemSelectionChanged(NodeViewModel nodeItem)
        {
            if (nodeItem == null) return;
            if (CurrentForestItem == null) return;
            SelectedNodeItem = nodeItem;
            NodeHelper.HideAllLabelColor(CurrentForestItem);

            if (BranchesSwitch)
            {
                
                NodeHelper.ShowSubLabelColor(nodeItem);
                Mediator.Broadcast("OnBranchItemSelectionChanged", SelectedNodeItem); // to visualizer
            }
            else
            {
                
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                Mediator.Broadcast("OnBuildupItemSelectionChanged", SelectedNodeItem); // to visualizer
            }
            CurrentForestItem.NotifyLabelColorChange();

            //UpdateLiveVisualization();
        }

        public void ColorNodesToBuildup()
        {
            if (CurrentForestItem == null) return;
            store.ForestSelected.SetBranchColorsBy("buildups");
            CurrentForestItem.NotifyLabelColorChange();
            DeselectNodes();
        }

        public void ColorNodesToBranch()
        {
            if (CurrentForestItem == null) return;
            store.ForestSelected.SetBranchColorsBy("branches");
            CurrentForestItem.NotifyLabelColorChange();
            DeselectNodes();
        }

        public void DeselectNodes()
        {
            if (CurrentForestItem == null) return;
            NodeHelper.HideAllLabelColor(CurrentForestItem);
            //EventMessenger.SendMessage("DeselectTreeView"); //send command to the view to deselect treeview
            ViewMediator.Broadcast("ViewDeselectTreeView"); //send command to the view to deselect treeview
            SelectedNodeItem = null;
            CurrentForestItem.NotifyLabelColorChange(); //better ways to do this?
            Mediator.Broadcast("TreeViewDeselected", CurrentForestItem); // to the visualizer
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
