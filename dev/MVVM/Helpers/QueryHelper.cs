using Calc.Core;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.MVVM.Helpers
{
    internal class QueryHelper
    {
        /// <summary>
        /// Creates calc elements from revit elements and sort them into queries.
        /// </summary>
        /// <returns>The leftover query set (performed with left over elements)</returns>
        internal static async Task<QueryTemplate> PerformQueriesAsync(QueryTemplate qryTemplate, IElementCreator elementCreator, List<CustomParamSetting> customParamSettings)
        {
            List<string> parameters = GetParameterList(qryTemplate);

            List<CalcElement> calcElements = await Task.Run(() => elementCreator.CreateCalcElements(customParamSettings, parameters));

            var leftElements = qryTemplate.PerformQueries(calcElements);
            var leftoverQuerySet = CreateLeftoverQuerySet("Unassigned", leftElements);

            return leftoverQuerySet;
        }

        /// <summary>
        /// creates a query set with all left over elements
        /// queries are conditioned by the categories
        /// branches are split by the type names
        /// make new query set and new queries for each category
        /// </summary>
        private static QueryTemplate CreateLeftoverQuerySet(string name, List<CalcElement> calcElements)
        {
            QueryTemplate leftoverQuerySet = new QueryTemplate() { Name = name, IsLeftover = true, Queries = new List<Query>() };
            //create queries for each category
            foreach (var category in calcElements.Select(e => e.Category).Distinct())
            {
                Query query = Query.CreateCategoryQuery(category);
                leftoverQuerySet.Queries.Add(query);
            }
            leftoverQuerySet.PerformQueries(calcElements);
            return leftoverQuerySet;
        }

        private static List<string> GetParameterList(QueryTemplate qryTemplate)
        {
            //get the root parameters from each query
            List<string>  parameters = qryTemplate.GetAllParameters();
            List<string> validatedParameters = ParameterHelper.ValidateParameterNames(parameters);
            // TODO: should show the illegal parameters
            Debug.WriteLine("Illegal parameter count: " + (parameters.Count - validatedParameters.Count));
            return validatedParameters;
        }
    }
}
