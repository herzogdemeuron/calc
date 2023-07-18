using Autodesk.Revit.UI;
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
        public MappingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }
        public void HandleNewMapping()
        {
            Window newMappingWindow = new NewMappingView(store);
            newMappingWindow.ShowDialog();
            Mediator.Broadcast("MappingSelectionChanged",store.MappingSelected);
        }
        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            Mediator.Broadcast("MappingSelectionChanged", mapping);
        }

        public async Task HandleUpdateMapping()
        {
            try
            {
                await store.UpdateSelectedMapping();
                TaskDialog.Show("Update Mapping", "Mapping updated"); // not topmost, fix it
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Update Mapping", ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
