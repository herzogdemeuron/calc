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
using Calc.ConnectorRevit.Services;

namespace Calc.ConnectorRevit.ViewModels
{
    public class ForestViewModel : INotifyPropertyChanged
    {
        private readonly DirectusStore store;        
        public ForestViewModel(DirectusStore directusStore)
        {
            store = directusStore;
            //Mediator.Register("ProjectSelected", _ => HandleForestSelectionChanged());
        }
        public void HandleForestSelectionChanged(Forest forest = null)
        {
            if (forest != null)
            {
                // if a forest is given, plant it and set it as the current forest
                store.ForestSelected = forest;
                
            }
            else if (store.ForestSelected != null)
            {
                // if no forest is given, replant the current forest
                forest = store.ForestSelected;
            }
            else
            {
                return;
            }

            foreach (var t in forest.Trees)
            {
                t.Plant(ElementFilter.GetCalcElements(t));
                //take out t.Elements from the baket

            }

            //ForestHelper.PlantTrees(forest);
            Mediator.Broadcast("ForestSelectionChanged");
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


