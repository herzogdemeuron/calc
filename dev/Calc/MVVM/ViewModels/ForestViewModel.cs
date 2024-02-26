using Calc.MVVM.Helpers;
using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.ComponentModel;
using Calc.Core.Interfaces;

namespace Calc.MVVM.ViewModels
{
    public class ForestViewModel : INotifyPropertyChanged
    {
        private readonly DirectusStore store;
        private IElementCreator elementCreator;
        public ForestViewModel(DirectusStore directusStore, IElementCreator elementCreator)
        {
            store = directusStore;
            this.elementCreator = elementCreator;
        }
        public void HandleForestSelectionChanged(Forest forest)
        {
            if (forest != null)
            {
                Mapping mapping;
                if (store.ForestSelected == forest)
                {
                    // if the same forest is selected, update the current forest presering the mapping
                    Mapping currentMapping = new Mapping("CurrentMapping", store.ForestSelected);
                    ForestHelper.PlantTrees(store.ForestSelected, elementCreator);
                    mapping = currentMapping;
                }
                else
                {
                    // otherwise create new forest and reset mapping
                    store.ForestSelected = forest;
                    ForestHelper.PlantTrees(forest, elementCreator);
                    mapping = store.MappingSelected;
                }
                MediatorFromVM.Broadcast("ForestSelectionChanged", mapping);
            }
        }

        public void HandleUpdateCurrentForest(Forest forest)
        {
            if (forest != null)
            {
                Mapping currentMapping = new Mapping("CurrentMapping", store.ForestSelected);
                ForestHelper.PlantTrees(store.ForestSelected, elementCreator);
                store.MappingSelected = currentMapping;
                MediatorFromVM.Broadcast("ForestSelectionChanged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


