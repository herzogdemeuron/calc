using Calc.ConnectorRevit.Services;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

            var leftElements = forest.PlantTrees(calcElements);
            watch.Stop();
            elapsedMs = watch.ElapsedMilliseconds;
            Debug.WriteLine("Time to plant trees: " + elapsedMs);
            Debug.WriteLine("Left overs: " + leftElements);
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
