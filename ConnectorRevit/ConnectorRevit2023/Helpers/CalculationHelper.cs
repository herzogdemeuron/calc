using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Calculations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.Objects;

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
