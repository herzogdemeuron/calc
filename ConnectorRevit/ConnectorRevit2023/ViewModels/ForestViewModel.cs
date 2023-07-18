﻿using Calc.ConnectorRevit.Views;
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
        public void HandleForestSelectionChanged(Forest forest)
        {
            if (forest != null)
            {
                Mapping mapping;
                if (store.ForestSelected == forest)
                      {
                    // if the same forest is selected, update the current forest presering the mapping
                    Mapping currentMapping = new Mapping(store.ForestSelected, "CurrentMapping");
                    ForestHelper.PlantTrees(store.ForestSelected);
                    mapping = currentMapping;
                }
                else
                {
                    // otherwise create new forest and reset mapping
                    store.ForestSelected = forest;
                    ForestHelper.PlantTrees(forest);
                    mapping = store.MappingSelected;
                }
                Mediator.Broadcast("ForestSelectionChanged", mapping);
            }
        }

        public void HandleUpdateCurrentForest(Forest forest)
        {
            if (forest != null)
            {
                bool a = store.ForestSelected == forest;
                Mapping currentMapping = new Mapping(store.ForestSelected, "CurrentMapping");
                ForestHelper.PlantTrees(store.ForestSelected);
                store.MappingSelected = currentMapping;
                Mediator.Broadcast("ForestSelectionChanged");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


