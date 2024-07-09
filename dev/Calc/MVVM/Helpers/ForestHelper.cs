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

    public class ForestHelper
    {
        /// <summary>
        /// creates calc elements from revit elements and sort them into trees
        /// return the dark forest (planted with left over elements)
        /// </summary>
        public static async Task<Forest> PlantTreesAsync(Forest forest, IElementCreator elementCreator, List<CustomParamSetting> customParamSettings)
        {
            List<string> parameters = GetParameterList(forest);

            List<CalcElement> calcElements = await Task.Run(() => elementCreator.CreateCalcElements(customParamSettings, parameters));

            var leftElements = forest.PlantTrees(calcElements);
            var darkForest = CreateDarkForest("Leftovers", leftElements);

            return darkForest;
        }

        /// <summary>
        /// creates a forest with all left over elements
        /// trees are conditioned by the categories
        /// branches are split by the type names
        /// make new forest and new trees for each category
        /// </summary>
        public static Forest CreateDarkForest(string name, List<CalcElement> calcElements)
        {

            Forest darkForest = new Forest() { Name = name, IsDark = true, Trees = new List<Tree>() };

            //create trees for each category
            foreach (var category in calcElements.Select(e => e.Category).Distinct())
            {
                Tree tree = MakeCategoryTree(category, darkForest);
                darkForest.Trees.Add(tree);
            }

            darkForest.PlantTrees(calcElements);

            return darkForest;
        }

        /// <summary>
        /// make a tree for a category
        /// </summary>
        private static Tree MakeCategoryTree(string categoryName, Forest forest)
        {
            Tree tree = new Tree() { ParentForest = forest };
            tree.Name = categoryName;

            SimpleCondition condition = new SimpleCondition() 
            { 
                Method = "equals", 
                Parameter = "Category", 
                Value = categoryName 
            };
            ConditionContainer conditionContainer = new ConditionContainer() 
            { 
                Type = "SimpleCondition", 
                Condition = condition 
            };
            tree.FilterConfig = new GroupCondition() 
            { 
                Conditions = new List<ConditionContainer>() { conditionContainer }, 
                Operator = "and" 
            };
            tree.BranchConfig = new List<string>() { "type:Type Name" };

            return tree;
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
