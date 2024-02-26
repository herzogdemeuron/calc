using Autodesk.Revit.DB;
using Calc.MVVM.Config;
using Calc.MVVM.Services;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Calc.MVVM.Helpers
{

    public class ForestHelper
    {
        /// <summary>
        /// creates calc elements from revit elements and sort them into trees
        /// </summary>
        public static void PlantTrees(Forest forest)
        {
            List<string> parameters = GetParameterList(forest);

            var doorParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Doors, AreaName: ".Standard_Area");
            var windowParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Windows, AreaName: ".Area");
            List<CalcElement> calcElements = Services.ElementFilter.CreateCalcElements(parameters, doorParamConfig, windowParamConfig);
            var leftElements = forest.PlantTrees(calcElements);

            //Debug.WriteLine("Left overs: " + leftElements);
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
