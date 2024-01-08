using Calc.ConnectorRevit.ViewModels;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;

namespace Calc.ConnectorRevit.Helpers
{
    public class MappingHelper
    {
        public static void ApplyMappingToForestItem(NodeViewModel ForestItem,DirectusStore store, Mapping newMapping, int maxBuildups)
        {
            foreach (NodeViewModel nodeItem in ForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                if (newMapping == null) continue;
                newMapping.ApplyToTree(tree, store.BuildupsAll, maxBuildups);
            };
        }

        public static Mapping CopyCurrentMapping(DirectusStore store)
        {
            return new Mapping(store.ForestSelected, "CurrentMapping");
        }

    }
}
