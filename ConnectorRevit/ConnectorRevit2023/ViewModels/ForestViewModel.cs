using Calc.ConnectorRevit.Views;
using Calc.Core.Objects;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Helpers;
using System.Collections.ObjectModel;

namespace Calc.ConnectorRevit.ViewModels
{
    public class ForestViewModel : INotifyPropertyChanged
    {
        private readonly DirectusStore store;        
        public ForestViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }
        public void HandleForestSelectionChanged(Forest forest)
        {
            store.ForestSelected = forest;
            ForestHelper.PlantTrees(forest);
            Mediator.Broadcast("ForestSelectionChanged");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


