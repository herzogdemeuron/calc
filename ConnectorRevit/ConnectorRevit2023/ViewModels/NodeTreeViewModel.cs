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
        private RevitVisualizer Visualizer;
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
            Mediator.Register("ForestSelectionChanged", _ => UpdateNodeSource());
            Mediator.Register("MappingSelectionChanged", _ => RemapAllNodes());
            Mediator.Register("BuildupSelectionChanged", _ => RecolorAllNodes());
            Mediator.Register("BuildupInherited", _ => RecolorAllNodes());
            Mediator.Register("UpdateCalcElements", _=> RemapAllNodes());

            Mediator.Register("ViewToggleToBuildup", _ => BranchesSwitch = false);
            Mediator.Register("ViewToggleToBuildup", _ => ColorNodesToBuildup());

            Mediator.Register("ViewToggleToBranch", _ => BranchesSwitch = true);
            Mediator.Register("ViewToggleToBranch", _ => ColorNodesToBranch());
            Visualizer = new RevitVisualizer();

            //changing priority:
            //Forest => Mapping => Buildup
            //server = new CalcWebSocketServer("http://127.0.0.1:8184/"); // to be moved to separate class
            //_ = server.Start(); // to be moved to separate class
        }

        public void UpdateNodeSource()
        {
            CurrentForestItem = new NodeViewModel(store.ForestSelected);
           /* if (store.ForestSelected != null)
            { 
                Visualizer = new RevitVisualizer(); 
            }*/
            OnPropertyChanged(nameof(NodeSource));
            DeselectNodes();
            //Mediator.Broadcast("MappingSelectionChanged");
        }
        public void RemapAllNodes()
        {
            MappingHelper.ApplySelectedMapping(CurrentForestItem, store);
            //Mediator.Broadcast("BuildupSelectionChanged");
        }

        private void RecolorAllNodes()
        {
            if (BranchesSwitch == false)
            {
                if (CurrentForestItem == null) return;
                store.ForestSelected.SetBranchColorsBy("buildups");
                CurrentForestItem.NotifyLabelColorChange();
                Mediator.Broadcast("AllNodesRecolored", SelectedNodeItem); // to visualizer
            }
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
                //eventHandler.Raise(Visualizer.IsolateAndColorSubbranchElements);
            }
            else
            {
                
                NodeHelper.ShowAllSubLabelColor(nodeItem);
                Mediator.Broadcast("OnBuildupItemSelectionChanged", SelectedNodeItem); // to visualizer
                //eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
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
            EventMessenger.SendMessage("DeselectTreeView"); //send command to the view to deselect treeview
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
