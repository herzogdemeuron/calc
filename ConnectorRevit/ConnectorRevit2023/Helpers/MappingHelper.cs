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
        public static void ApplyMappingToForestItem(NodeViewModel ForestItem,DirectusStore store, Mapping newMapping)
        {
            foreach (NodeViewModel nodeItem in ForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                if (newMapping == null) continue;
                newMapping.ApplyMappingToTree(tree, store.BuildupsAll);
            };
            
        }

        public static Mapping CopyCurrentMapping(DirectusStore store)
        {
            return new Mapping(store.ForestSelected, "CurrentMapping");
        }

    }
}
