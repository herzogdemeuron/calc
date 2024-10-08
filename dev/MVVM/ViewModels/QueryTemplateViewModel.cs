using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    internal class QueryTemplateViewModel
    {
        private readonly CalcStore store;
        private readonly IElementCreator elementCreator;
        private readonly VisibilityViewModel visibilityVM;
        public QueryTemplateViewModel(CalcStore calcStore, IElementCreator elementCreator, VisibilityViewModel vvm)
        {
            store = calcStore;
            this.elementCreator = elementCreator;
            this.visibilityVM = vvm;
        }

        /// <summary>
        /// Sets the new selected query template to store, perform queries and update the leftover query set
        /// </summary>
        internal async Task HandleQueryTemplateSelectionChanged(QueryTemplate qryTemplate)
        {
            if (qryTemplate == null) return;
            try
            {
                store.QueryTemplateSelected = qryTemplate;
                store.LeftoverQuerySet = await QueryHelper.PerformQueriesAsync(qryTemplate, elementCreator, store.CustomParamSettingsAll);
            }
            catch (System.Exception e)
            {
                visibilityVM.ShowMessageOverlay(null, e.Message);
                store.QueryTemplateSelected = null;
            }
        }

    }
}