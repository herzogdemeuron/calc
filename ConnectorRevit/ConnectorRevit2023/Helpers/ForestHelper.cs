using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Services;
using Calc.Core.Filtering;
using System.Diagnostics;

namespace Calc.ConnectorRevit.Helpers
{
    public class ForestHelper
    {
        public static void PlantTrees(Forest forest)
        {
            List<string> parameters = GetParameterList(forest);
            List<CalcElement> calcElements = RevitElementFilter.CreateCalcElements(parameters);
            foreach (var t in forest.Trees)
            {
                calcElements = t.Plant(calcElements);
            }
            Debug.WriteLine("Left overs: " + calcElements.Count);
        }

        private static List<string> GetParameterList(Forest forest)
        {
            //get the root parameters from each tree
            List<string> parameters = new List<string>();
            foreach (var t in forest.Trees)
            {
                parameters.AddRange(t.FilterConfig.GetAllParameters());
            }
            List<string> validatedParameters = ParameterHelper.ValidateParameterNames(parameters);
            // TODO: should show the illegal parameters
            Debug.WriteLine("Illegal parameter count: " + (parameters.Count - validatedParameters.Count));
            return validatedParameters;
        }
    }
}
