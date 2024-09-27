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
    public class ForestViewModel
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
            if (forest == null) return;
            try
            {
                store.ForestSelected = forest;
                store.DarkForestSelected = await ForestHelper.PlantTreesAsync(forest, elementCreator, store.CustomParamSettingsAll);
            }
            catch (System.Exception e)
            {
                // show error message if something goes wrong
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, e.Message });
                store.ForestSelected = null;
            }
        }

    }
}


