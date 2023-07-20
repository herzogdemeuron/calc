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


        public async Task<bool?> HandleSaveResults(string newName)
        {
            NodeViewModel CurrentForestItem = nodetreeVM.CurrentForestItem;
            NodeViewModel SelectedNodeItem = nodetreeVM.SelectedNodeItem;
            if (CurrentForestItem == null) return null;
            NodeViewModel nodeToCalculate = SelectedNodeItem ?? CurrentForestItem;
            ElementCount = nodeToCalculate.Host.Elements.Count;
            ViewMediator.Broadcast("VisibilitySaving");
            ResultSender resultSender = new ResultSender();
            return await resultSender.SaveResults(nodeToCalculate, newName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
