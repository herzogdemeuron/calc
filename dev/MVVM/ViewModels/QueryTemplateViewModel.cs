using Calc.MVVM.Helpers;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Calc.MVVM.ViewModels
{
    public class QueryTemplateViewModel
    {
        private readonly CalcStore store;
        private IElementCreator elementCreator;
        private readonly VisibilityViewModel visibilityVM;
        public QueryTemplateViewModel(CalcStore calcStore, IElementCreator elementCreator, VisibilityViewModel vvm)
        {
            store = calcStore;
            this.elementCreator = elementCreator;
            this.visibilityVM = vvm;
        }

        /// <summary>
        /// Set the new selected query template to store, plant queries and update the black query set
        /// </summary>
        public async Task HandleQueryTemplateSelectionChanged(QueryTemplate qryTemplate)
        {
            if (qryTemplate == null) return;
            try
            {
                store.QueryTemplateSelected = qryTemplate;
                store.BlackQuerySet = await QueryHelper.PlantQueriesAsync(qryTemplate, elementCreator, store.CustomParamSettingsAll);
            }
            catch (System.Exception e)
            {
                visibilityVM.ShowMessageOverlay(null, e.Message);
                store.QueryTemplateSelected = null;
            }
        }

    }
}


