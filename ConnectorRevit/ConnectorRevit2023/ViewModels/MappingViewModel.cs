using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get => store.MappingSelected;
            set
            {
                store.MappingSelected = value;
                OnPropertyChanged(nameof(SelectedMapping));
            }
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            //MappingSelected = mapping;
            if (CurrentForestItem == null)
                return;
            ApplyMapping(mapping);
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
