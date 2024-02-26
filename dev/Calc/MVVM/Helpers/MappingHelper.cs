using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.Collections.Generic;
using Calc.MVVM.Models;

namespace Calc.MVVM.Helpers
{
    public class MappingHelper
    {
        public static Forest ApplyMappingToForestItem(NodeModel ForestItem,DirectusStore store, Mapping newMapping, int maxBuildups)
        {
            var brokenForest = new Forest()
            {
                Name = ForestItem.Name,
                Trees = new List<Tree>()
            };
            foreach (NodeModel nodeItem in ForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                if (newMapping == null) continue;
                var brokenTree = newMapping.ApplyToTree(tree, store.BuildupsAll, maxBuildups);
                if (brokenTree != null && brokenTree.SubBranches.Count > 0)
                {
                    brokenForest.Trees.Add(brokenTree);
                }
            };
            return brokenForest;
        }

        public static Mapping CopyCurrentMapping(DirectusStore store)
        {
            return new Mapping("CurrentMapping", store.ForestSelected);
        }

    }
}
