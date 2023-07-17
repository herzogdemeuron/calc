using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Revit;
using Calc.ConnectorRevit.Services;
using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NodeTreeViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        private bool BranchesSwitch { get; set;}
        private readonly ExternalEventHandler eventHandler = new ExternalEventHandler();

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
        public ObservableCollection<NodeViewModel> NodeSource { get => new ObservableCollection<NodeViewModel> { CurrentForestItem }; }

        public NodeTreeViewModel(DirectusStore directusStore, bool branchSwitch)
        {
            store = directusStore;
            BranchesSwitch = branchSwitch;
            Mediator.Register("ForestSelectionChanged", _ => UpdateNodeSource());
            Mediator.Register("MappingSelectionChanged", _ => RemapAllNodes());
            Mediator.Register("BuildupSelectionChanged", _ => ReColorNodes());
            Mediator.Register("BuildupInherited", _ => ReColorNodes());
            Mediator.Register("UpdateCalcElements", _=> RemapAllNodes());

            Mediator.Register("ViewToggleToBuildup", _ => ColorNodesToBuildup());
            Mediator.Register("ViewToggleToBranch", _ => ColorNodesToBranch());

            //changing priority:
            //Forest => Mapping => Buildup
            //server = new CalcWebSocketServer("http://127.0.0.1:8184/"); // to be moved to separate class
            //_ = server.Start(); // to be moved to separate class
        }
        private void ReColorNodes()
        {
            if (CurrentForestItem == null) return;
            store.ForestSelected.SetBranchColorsBy("buildups");
            CurrentForestItem.NotifyLabelColorChange();
            eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements); //to be moved to visualizer class
        }
        public void UpdateNodeSource()
        {
            CurrentForestItem = new NodeViewModel(store.ForestSelected);
            OnPropertyChanged(nameof(NodeSource));
            //HandleSideClick();
            //HandleBuildupSelectionChanged();
        }
        public void RemapAllNodes()
        {
            MappingHelper.ApplySelectedMapping(CurrentForestItem, store);
        }
        public void HandleNodeItemSelectionChanged(NodeViewModel nodeItem)
        {
            if (nodeItem == null) return;
            if (CurrentForestItem == null) return;
            SelectedNodeItem = nodeItem;
            NodeHelper.HideAllLabelColor(CurrentForestItem);

            if (BranchesSwitch)
            {
                eventHandler.Raise(Visualizer.IsolateAndColorSubbranchElements);
                NodeHelper.ShowSubLabelColor(nodeItem);
            }
            else
            {
                eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
                NodeHelper.ShowAllSubLabelColor(nodeItem);
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
            eventHandler.Raise(Visualizer.Reset);
            NodeHelper.HideAllLabelColor(CurrentForestItem);
            EventMessenger.SendMessage("DeselectTreeView");
            SelectedNodeItem = null;
            CurrentForestItem.NotifyLabelColorChange(); //better ways to do this?
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
