using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Config;
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

            var doorParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Doors, AreaName: ".Standard_Area");
            var windowParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Windows, AreaName: ".Area");
            List<CalcElement> calcElements = RevitElementFilter.CreateCalcElements(parameters, doorParamConfig, windowParamConfig);

            var leftElements = forest.PlantTrees(calcElements);

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
