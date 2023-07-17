using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calc.ConnectorRevit.ViewModels
{
    public class MappingViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        public List<Mapping> CurrentMappings
        {
            get => store?.MappingsProjectRelated;
        }
        public Mapping SelectedMapping
        {
            get => store?.MappingSelected;
            set
            {
                store.MappingSelected = value;
                OnPropertyChanged(nameof(SelectedMapping));
            }
        }

        public MappingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }
        public void HandleNewMapping()
        {
            Window newMappingWindow = new NewMappingView(store);
            newMappingWindow.ShowDialog();
            OnPropertyChanged(nameof(CurrentMappings));
            OnPropertyChanged(nameof(SelectedMapping));//needed?

        }
        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            //store.MappingSelected = mapping;
            Mediator.Broadcast("MappingSelectionChanged");
        }

        public void HandleUpdateMapping()
        {
            _ = Task.Run(async () => await store.UpdateSelectedMapping());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
