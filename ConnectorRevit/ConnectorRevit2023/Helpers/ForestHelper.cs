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
            foreach (var t in forest.Trees)
            {
                t.Plant(ElementFilter.GetCalcElements(t));
            }
        }
    }
}
