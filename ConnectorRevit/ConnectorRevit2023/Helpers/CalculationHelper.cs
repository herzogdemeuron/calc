using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Calculations;
using Calc.Core.Objects;
using System.Collections.Generic;

namespace Calc.ConnectorRevit.Helpers
{
    class CalculationHelper
    {

        public static List<Result> Calculate(NodeViewModel nodeItem)
        {
            List<Branch> branchesToCalc = new List<Branch>();

            if (nodeItem.Host is Branch branch)
            {
                branchesToCalc.Add(branch);
            }
            else
            // if the node is a forest, add the branches(trees) of the forest
            {
                foreach (NodeViewModel subItem in nodeItem.SubNodeItems)
                {
                    branchesToCalc.Add(subItem.Host as Branch);
                }
            }

            List<Result> results = Calculator.Calculate(branchesToCalc);
            return results;
        }
    }
}
