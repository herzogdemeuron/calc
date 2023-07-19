using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Services;

namespace Calc.ConnectorRevit.Helpers
{
    public class ForestHelper
    {
        public static void PlantTrees(Forest forest)
        {
            List<string> parameters = GetParameterList(forest);
            List<string> BacketElementIds = new List<string>();
            foreach (var t in forest.Trees)
            {
                //List<CalcElement> calcElements = ElementFilter.GetCalcElements(t, parameters, createdRevitElementIds);
                List<CalcElement> calcElements = ElementFilter.GetCalcElements(t);
                t.Plant(calcElements);
            }
        }

        private static List<string> GetParameterList(Forest forest)
        {
            //get the root parameters from each tree
            var parameters = new List<string>();
            foreach (var t in forest.Trees)
            {
               List<Root> roots = t.Roots;
                foreach (var r in roots)
                {
                    if (!parameters.Contains(r.Parameter))
                    {
                        parameters.Add(r.Parameter);
                    }
                }
            }
            return parameters;
        }
    }
}
