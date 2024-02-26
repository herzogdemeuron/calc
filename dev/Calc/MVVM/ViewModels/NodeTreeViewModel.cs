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

namespace Calc.MVVM.ViewModels
{
    public class NodeTreeViewModel : INotifyPropertyChanged
    {
        public DirectusStore Store;
        public  List<Buildup> AllBuildups => Store.BuildupsAll;
        public int MaxBuildups { get; set; } = 2;
        public bool BranchesSwitch { get; set; }        

        private NodeViewModel selectedNodeItem;
        private IVisualizer visualizer;
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
        public NodeViewModel CurrentBrokenForestItem { get; set; }

        public ObservableCollection<NodeViewModel> NodeSource 
        { get => new ObservableCollection<NodeViewModel> { CurrentForestItem }; }

        public NodeTreeViewModel(DirectusStore directusStore, IVisualizer visualizer)
        {
            Store = directusStore;
            this.visualizer = visualizer;
            BranchesSwitch = true;
            MediatorFromVM.Register("ForestSelectionChanged", mapping => UpdateNodeSource((Mapping)mapping));
            MediatorFromVM.Register("MappingSelectionChanged", mapping => RemapAllNodes((Mapping)mapping));
            MediatorFromVM.Register("BuildupSelectionChanged", _ => RecolorAllNodes());

            MediatorFromVM.Register("MainViewToggleToBuildup", _ => BranchesSwitch = false);
            MediatorFromVM.Register("MainViewToggleToBuildup", _ => ColorNodesToBuildup());

            MediatorFromVM.Register("MainViewToggleToBranch", _ => BranchesSwitch = true);
            MediatorFromVM.Register("MainViewToggleToBranch", _ => ColorNodesToBranch());
            new ResultSender();
            //changing priority: Forest => Mapping => Buildup
        }


        public void UpdateNodeSource(Mapping mapping)
        {
            CurrentForestItem = new NodeViewModel(Store.ForestSelected, this);
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
                if(forceRecolorAll)
                {
                    Store.ForestSelected.SetBranchColorsBy("branches");
                    visualizer.IsolateAndColorSubbranchElements(SelectedNodeItem.Host);
                }
            }
            else
            {
                Store.ForestSelected.SetBranchColorsBy("buildups");
                visualizer.IsolateAndColorBottomBranchElements(SelectedNodeItem.Host);
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
                visualizer.IsolateAndColorSubbranchElements(SelectedNodeItem.Host);
            }
            else
            {
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                visualizer.IsolateAndColorBottomBranchElements(SelectedNodeItem.Host);
            }

            CurrentForestItem.NotifyNodePropertyChange();
            MediatorFromVM.Broadcast("NodeItemSelectionChanged");
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
            MediatorToView.Broadcast("ViewDeselectTreeView"); //send command to the view to deselect treeview
            SelectedNodeItem = null;
            CurrentForestItem.NotifyNodePropertyChange(); //better ways to do this?
            MediatorFromVM.Broadcast("NodeItemSelectionChanged");
            visualizer.ResetView(CurrentForestItem.SubNodeItems.Select(x => x.Host).ToList());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
