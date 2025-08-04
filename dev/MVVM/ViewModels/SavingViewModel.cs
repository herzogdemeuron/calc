using Calc.MVVM.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// VM for the saving dialog.
    /// </summary>
    public class SavingViewModel
    {
        private readonly CalculationViewModel calculationVM;
        private readonly VisibilityViewModel visibilityVM;

        public SavingViewModel(CalculationViewModel calVM, VisibilityViewModel vvm)
        
        {
            calculationVM = calVM;
            visibilityVM = vvm;
        }

        /// <summary>
        /// The saving confirmation.
        /// </summary>
        internal void HandleSavingResults()
        {
            int count = calculationVM.AssemblySnapshots?
                .Sum(assembly => assembly.ElementTypes?
                .Sum(elType => elType.ElementIds?.Count 
                ?? 0)?? 0)?? 0;

            string message;
            message = $"{count} elements to save.";            
            visibilityVM.ShowSavingOverlay(message);
        }

        /// <summary>
        /// The saving and save result message.
        /// </summary>
        internal async Task HandleSendingResults(string newName)
        {
            visibilityVM.ShowWaitingOverlay("Saving results...");
            var feedback =  await SnapshotSender.SaveProjectSnapshot(newName, calculationVM);
            bool? saved = feedback.Item1;
            string error = feedback.Item2;
            visibilityVM.HideAllOverlays();
            visibilityVM.ShowMessageOverlay(
                        saved,
                        "No element saved",
                        "Saved results successfully.",
                        error??"Error occured while saving."
                        );
        }

        internal void HandleSaveResultCanceled()
        {
            visibilityVM.HideAllOverlays();
        }

    }
}
