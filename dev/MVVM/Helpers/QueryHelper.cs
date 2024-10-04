using Calc.Core.Filtering;
using Calc.Core.Interfaces;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.MVVM.Helpers
{

    public class QueryHelper
    {
        /// <summary>
        /// creates calc elements from revit elements and sort them into queries
        /// return the black query set (planted with left over elements)
        /// </summary>
        public static async Task<QueryTemplate> PlantQueriesAsync(QueryTemplate qryTemplate, IElementCreator elementCreator, List<CustomParamSetting> customParamSettings)
        {
            List<string> parameters = GetParameterList(qryTemplate);

            List<CalcElement> calcElements = await Task.Run(() => elementCreator.CreateCalcElements(customParamSettings, parameters));

            var leftElements = qryTemplate.PlantQueries(calcElements);
            var blackQuerySet = CreateBlackQuerySet("Unassigned", leftElements);

            return blackQuerySet;
        }

        /// <summary>
        /// creates a query set with all left over elements
        /// queries are conditioned by the categories
        /// branches are split by the type names
        /// make new query set and new queries for each category
        /// </summary>
        private static QueryTemplate CreateBlackQuerySet(string name, List<CalcElement> calcElements)
        {
            QueryTemplate blackQuerySet = new QueryTemplate() { Name = name, IsBlack = true, Queries = new List<Query>() };
            //create queries for each category
            foreach (var category in calcElements.Select(e => e.Category).Distinct())
            {
                Query query = Query.MakeCategoryQuery(category);
                blackQuerySet.Queries.Add(query);
            }
            blackQuerySet.PlantQueries(calcElements);
            return blackQuerySet;
        }

        private static List<string> GetParameterList(QueryTemplate qryTemplate)
        {
            //get the root parameters from each query
            List<string>  parameters = qryTemplate.GetAllParameters().ToList();
            List<string> validatedParameters = ParameterHelper.ValidateParameterNames(parameters);
            // TODO: should show the illegal parameters
            Debug.WriteLine("Illegal parameter count: " + (parameters.Count - validatedParameters.Count));
            return validatedParameters;
        }
    }
}
