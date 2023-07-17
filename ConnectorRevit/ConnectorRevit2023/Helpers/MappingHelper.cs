using Calc.ConnectorRevit.ViewModels;
using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Helpers
{
    public class MappingHelper
    {
        public static void ApplySelectedMapping(NodeViewModel ForestItem,DirectusStore store)
        {
            Mapping mapping = store.MappingSelected;
            foreach (NodeViewModel nodeItem in ForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);

                if (mapping == null)
                    continue;
                mapping.ApplyMappingToTree(tree, store.BuildupsAll);
            };
            
        }

        public static Mapping CopyCurrentMapping(DirectusStore store)
        {
            string name = store.MappingSelected.Name;
            return new Mapping(store.ForestSelected, "CurrentMapping");
        }

    }
}
