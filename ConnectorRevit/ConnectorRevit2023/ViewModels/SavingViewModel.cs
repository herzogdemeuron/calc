using Calc.Core.Objects;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

namespace Calc.ConnectorRevit.ViewModels
{

    public class SavingViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel nodetreeVM;
        public double ElementCount { get; set; }


        public SavingViewModel(NodeTreeViewModel ntreeVM)
        
        {
            nodetreeVM = ntreeVM;
        }

        public void HandleSavingResults()
        {
            NodeViewModel nodeToCalculate = GetNodeToCalculate();
            int count = nodeToCalculate?.Host.Elements.Count??0;
            string message;
            if (count>100)
            {
                message = $"{count} elements to save,\nthis may take a while.";
            }
            else
            {
                message = $"{count} elements to save.";
            }
            ViewMediator.Broadcast("ShowSavingOverlay", message);
        }


        public async Task HandleSendingResults(string newName)
        {
            NodeViewModel nodeToCalculate = GetNodeToCalculate();
            ViewMediator.Broadcast("ShowWaitingOverlay", "Saving results...");
            ResultSender resultSender = new ResultSender();
            bool? feedback =  await resultSender.SaveResults(nodeToCalculate, newName);
            ViewMediator.Broadcast
                ("ShowMessageOverlay",
                new List<object> 
                    {   feedback,
                        "No element is saved",
                        "Saved results successfully.",
                        "Error occured while saving." 
                    }
                 );
            ViewMediator.Broadcast("ShowMainView");
        }

        public void HandleSaveResultCanceled()
        {
            ViewMediator.Broadcast("ShowMainView");
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
