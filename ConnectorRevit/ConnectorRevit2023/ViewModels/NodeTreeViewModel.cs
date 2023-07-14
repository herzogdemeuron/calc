using Calc.ConnectorRevit.Helpers;
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
        public NodeViewModel CurrentForestItem { get; set; }
        public ObservableCollection<NodeViewModel> NodeSource { get => new ObservableCollection<NodeViewModel> { CurrentForestItem }; }

        public NodeTreeViewModel(DirectusStore directusStore)
        {
            store = directusStore;
            Mediator.Register("ForestSelectionChanged", _ => UpdateNodeSource());
            Mediator.Register("MappingSelectionChanged", _ => RemapAllNodes());
            //changing priority:
            //Forest => Mapping => Buildup
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
