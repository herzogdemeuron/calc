using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    internal class MappingViewModel : INotifyPropertyChanged
    {
        private readonly CalcStore store;
        private readonly VisibilityViewModel visibilityVM;
        public QueryTemplate BrokenQuerySet { get; set; }
        public MappingViewModel(CalcStore calcStore, VisibilityViewModel vvm)
        {
            store = calcStore;
            visibilityVM = vvm;
        }

        public async Task HandleUpdateMapping()
        {
            bool? feedback;
            string error = "";
            try
            {
                visibilityVM.ShowWaitingOverlay("Updating mapping...");
                feedback = await store.UpdateSelectedMapping(BrokenQuerySet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            visibilityVM.HideAllOverlays();
            visibilityVM.ShowMessageOverlay(
                        feedback,
                        error,
                        "Updated mapping successfully.",
                        "Error occured while saving, please try again."
                        );

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
