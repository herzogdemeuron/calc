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
            // record time
            var watch = System.Diagnostics.Stopwatch.StartNew();


            List<CalcElement> calcElements = RevitElementFilter.CreateCalcElements(parameters);
            
            // record time
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.WriteLine("Time to create CalcElements: " + elapsedMs);

            // record time
            watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var t in forest.Trees)
            {
                calcElements = t.Plant(calcElements);
            }
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Debug.WriteLine("Time to plant trees: " + elapsedMs);
            Debug.WriteLine("Left overs: " + calcElements.Count);
        }

        private static List<string> GetParameterList(Forest forest)
        {
            //get the root parameters from each tree
            List<string>  parameters = forest.GetAllParameters().ToList();
            List<string> validatedParameters = ParameterHelper.ValidateParameterNames(parameters);
            // TODO: should show the illegal parameters
            Debug.WriteLine("Illegal parameter count: " + (parameters.Count - validatedParameters.Count));
            return validatedParameters;
        }
    }
}
