using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.Collections.Generic;
using Calc.MVVM.Models;
using System.Linq;

namespace Calc.MVVM.Helpers
{
    public class MappingHelper
    {
        public static Forest ApplyMappingToForestItem(NodeModel ForestItem,CalcStore store, Mapping newMapping)
        {
            // todo: add missing mappings from missing trees to the dark forest
            var brokenForest = new Forest()
            {
                Name = ForestItem.Name,
                Trees = new List<Tree>()
            };
            foreach (NodeModel nodeItem in ForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                if (newMapping == null) continue;
                var verifiedBuildups = store.BuildupsAll.Where(b => b.Verified).ToList();
                var brokenTree = newMapping.ApplyToTree(tree, verifiedBuildups);
                if (brokenTree != null && brokenTree.SubBranches.Count > 0)
                {
                    brokenForest.Trees.Add(brokenTree);
                }
            };
            return brokenForest;
        }

        public static Mapping CopyCurrentMapping(CalcStore store)
        {
            return new Mapping("CurrentMapping", store.ForestSelected);
        }

    }
}
