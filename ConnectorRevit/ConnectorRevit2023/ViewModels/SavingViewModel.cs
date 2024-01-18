using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.ConnectorRevit.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{

    public class SavingViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel nodetreeVM;
        private readonly CalculationViewModel calculationVM;
        public double ElementCount { get; set; }


        public SavingViewModel(CalculationViewModel calVM)
        
        {
            calculationVM = calVM;
        }

        public void HandleSavingResults()
        {
            int count = calculationVM.Results?.Count??0;
            string message;
            if (count>100)
            {
                message = $"{count} elements to save,\nthis may take a while.";
            }
            else
            {
                message = $"{count} elements to save.";
            }
            MediatorToView.Broadcast("ShowSavingOverlay", message);
        }


        public async Task HandleSendingResults(string newName)
        {
            
            MediatorToView.Broadcast("ShowWaitingOverlay", "Saving results...");
            ResultSender resultSender = new ResultSender();
            bool? feedback =  await resultSender.SaveResults(calculationVM.Store,calculationVM.Results,newName);
            MediatorToView.Broadcast("ShowMainView");
            MediatorToView.Broadcast
                ("ShowMessageOverlay",
                new List<object> 
                    {   feedback,
                        "No element is saved",
                        "Saved results successfully.",
                        "Error occured while saving." 
                    }
                 );
        }

        public void HandleSaveResultCanceled()
        {
            MediatorToView.Broadcast("ShowMainView");
        }

        private NodeViewModel GetNodeToCalculate()
        {
            NodeViewModel CurrentForestItem = nodetreeVM.CurrentForestItem;
            NodeViewModel SelectedNodeItem = nodetreeVM.SelectedNodeItem;
            if (CurrentForestItem == null) return null;
            NodeViewModel nodeToCalculate = SelectedNodeItem ?? CurrentForestItem;
            return nodeToCalculate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
