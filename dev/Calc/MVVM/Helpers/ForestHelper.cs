using Calc.Core.Interfaces;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.MVVM.Helpers
{

    public class ForestHelper
    {
        /// <summary>
        /// creates calc elements from revit elements and sort them into trees
        /// </summary>
        public static async Task PlantTreesAsync(Forest forest, IElementCreator elementCreator, List<CustomParamSetting> customParamSettings)
        {
            List<string> parameters = GetParameterList(forest);

            List<CalcElement> calcElements = await Task.Run(() => elementCreator.CreateCalcElements(customParamSettings, parameters));

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
