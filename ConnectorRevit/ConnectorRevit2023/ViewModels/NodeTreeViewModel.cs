using Calc.ConnectorRevit.Helpers;
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
            CurrentForestItem = new NodeViewModel(store, store.ForestSelected, this);
            OnPropertyChanged(nameof(NodeSource));
            DeselectNodes();
            Mediator.Broadcast("MappingSelectionChanged", mapping);
        }
        public void RemapAllNodes(Mapping mapping)
        {
            if (CurrentForestItem == null) return;
            MappingHelper.ApplyMappingToForestItem(CurrentForestItem, store, mapping, MaxBuildups);
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        /// <summary>
        /// reset all node label colors property according to the current branch/buildup switch
        /// </summary>
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
            var host = nodeItem.Host as Branch;
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
            Mediator.Broadcast("NodeItemSelectionChanged");
            // TODO sort out the mediator broadcasts
            //Mediator.Broadcast("BuildupSelectionChanged");
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
            ViewMediator.Broadcast("ViewDeselectTreeView"); //send command to the view to deselect treeview
            SelectedNodeItem = null;
            CurrentForestItem.NotifyLabelColorChange(); //better ways to do this?
            Mediator.Broadcast("TreeViewDeselected", CurrentForestItem); // to the visualizer
            //Mediator.Broadcast("BuildupSelectionChanged");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
