using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            if (store.ForestSelected != null)
            {
                Mediator.Broadcast("MappingSelectionChanged", mapping);
            }
            
        }

        public async Task HandleUpdateMapping()
        {
            bool? feedback;
            string error = "";
            try
            {
                ViewMediator.Broadcast("ShowWaitingOverlay", "Updating mapping...");
                feedback = await store.UpdateSelectedMapping();
                ViewMediator.Broadcast("ShowMainView");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ViewMediator.Broadcast("ShowMainView");
                feedback = null;
                error = ex.Message;
            }
            ViewMediator.Broadcast
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
