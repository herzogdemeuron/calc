using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.Collections.Generic;
using Calc.MVVM.Models;
using System.Linq;

namespace Calc.MVVM.Helpers
{
    internal class MappingHelper
    {
        /// <summary>
        /// Applies a mapping to a query template.
        /// An unverified assembly will be marked to broken.
        /// </summary>
        /// <returns>The broken queries with missing mappings.</returns>
        internal static QueryTemplate ApplyMapping(NodeModel queryTemplateItem,CalcStore store, Mapping newMapping)
        {
            var brokenQueries = new QueryTemplate()
            {
                Name = queryTemplateItem.Name,
                Queries = new List<Query>()
            };
            foreach (NodeModel nodeItem in queryTemplateItem.SubNodeItems)
            {
                Query query = nodeItem.Host as Query;
                if (newMapping == null) continue;
                var verifiedAssemblies = store.AssembliesAll.Where(b => b.Verified).ToList();
                var brokenQuery = newMapping.ApplyToQuery(query, verifiedAssemblies);
                if (brokenQuery != null && brokenQuery.SubBranches.Count > 0)
                {
                    brokenQueries.Queries.Add(brokenQuery);
                }
            };
            return brokenQueries;
        }
    }
}
