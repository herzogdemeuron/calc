using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class MappingViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        public MappingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            if (store.ForestSelected != null)
            {
                MediatorFromVM.Broadcast("MappingSelectionChanged", mapping);
            }
        }

        public async Task HandleUpdateMapping()
        {
            bool? feedback;
            string error = "";
            try
            {
                MediatorToView.Broadcast("ShowWaitingOverlay", "Updating mapping...");
                feedback = await store.UpdateSelectedMapping();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            MediatorToView.Broadcast("ShowMainView");
            MediatorToView.Broadcast
               ("ShowMessageOverlay",
               new List<object>
                   {   feedback,
                        error,
                        "Updated mapping successfully.",
                        "Error occured while saving, please try again."
                   }
                );
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
