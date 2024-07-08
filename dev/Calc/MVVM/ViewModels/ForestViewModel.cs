using Calc.MVVM.Helpers;
using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.ComponentModel;
using Calc.Core.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calc.MVVM.ViewModels
{
    public class ForestViewModel : INotifyPropertyChanged
    {
        private readonly CalcStore store;
        private IElementCreator elementCreator;
        public ForestViewModel(CalcStore calcStore, IElementCreator elementCreator)
        {
            store = calcStore;
            this.elementCreator = elementCreator;
        }
        public async Task HandleForestSelectionChanged(Forest forest)
        {
            if (forest != null)
            {
                try
                {
                    Mapping mapping;
                    if (store.ForestSelected == forest)
                    {
                        // if the same forest is selected, update the current forest presering the mapping
                        Mapping currentMapping = new Mapping("CurrentMapping", store.ForestSelected);
                        var darkForest = await ForestHelper.PlantTreesAsync(store.ForestSelected, elementCreator, store.CustomParamSettingsAll);
                        store.DarkForestSelected = darkForest;
                        mapping = currentMapping;
                    }
                    else
                    {
                        // otherwise create new forest and reset mapping
                        store.ForestSelected = forest;
                        var darkForest = await ForestHelper.PlantTreesAsync(forest, elementCreator, store.CustomParamSettingsAll);
                        store.DarkForestSelected = darkForest;
                        mapping = store.MappingSelected;
                    }
                    MediatorFromVM.Broadcast("ForestSelectionChanged", mapping);
                }
                catch (System.Exception e)
                {
                    // show error message if something goes wrong
                    MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, e.Message });
                    store.ForestSelected = null;
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


