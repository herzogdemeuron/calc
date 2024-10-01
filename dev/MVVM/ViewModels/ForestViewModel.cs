using Calc.MVVM.Helpers;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calc.MVVM.ViewModels
{
    public class ForestViewModel
    {
        private readonly CalcStore store;
        private IElementCreator elementCreator;
        private readonly VisibilityViewModel visibilityVM;
        public ForestViewModel(CalcStore calcStore, IElementCreator elementCreator, VisibilityViewModel vvm)
        {
            store = calcStore;
            this.elementCreator = elementCreator;
            this.visibilityVM = vvm;
        }

        /// <summary>
        /// Set the new selected forest to store, plant trees and update the dark forest
        /// </summary>
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
                visibilityVM.ShowMessageOverlay(null, e.Message);
                store.ForestSelected = null;
            }
        }

    }
}


