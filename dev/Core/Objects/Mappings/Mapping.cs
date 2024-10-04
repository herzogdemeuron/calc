using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.GraphNodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calc.Core.Objects.Mappings
{
    public class Mapping : IHasProject
    {
        [JsonProperty("id")]
        public int Id { get; set; } = -1;
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("project")]
        public CalcProject Project { get; set; }
        [JsonProperty("updated")]
        public DateTime? Updated { get; set; }
        [JsonProperty("mappings")]
        public List<MappingItem> MappingItems { get; set; }

        public Mapping()
        {
            // empty constructor for deserialization
        }

        public Mapping(string mappingName, params QueryTemplate[] queryTemplates) // simplify to taking one template
        {
            MappingItems = new List<MappingItem>();
            foreach (var qt in queryTemplates)
            {
                if(qt == null)
                {
                    continue;
                }

                foreach (var query in qt.Queries)
                {
                    MappingItems.AddRange(ExtractMappingItems(query.Flatten(), query.Name));
                }
            }
            Name = mappingName;
        }

        public void AddMappingItems(List<MappingItem> mappingItems)
        {
            MappingItems.AddRange(mappingItems);
        }

        /// <summary>
        /// Assigns the assemblies to the query based on the mapping.
        /// Automatically determines which mapping to use based on the query name.
        /// Returns a broken query if the mapping path is not found.
        /// </summary>
        public Query ApplyToQuery(Query query, List<Assembly> allAssemblies)
        {
            var brokenQuery = new Query()
            {
                Name = query.Name,
                ParentQuery = query
            };

            query.ResetAssemblies();
            // find the mapping items that apply to this query
            var mappingItems = MappingItems.Where(mappingItem => mappingItem.QueryName == query.Name);
            if (!mappingItems.Any())
            {
                return null;
            }

            foreach (var mappingItem in mappingItems)
            {
                var assemblyIds = mappingItem.AssemblyIds.Take(2).ToList();
                var assemblies = allAssemblies.Where(b => assemblyIds.Contains(b.Id)).ToList();
                var match = MapAssembliesToBranch(query, assemblies, mappingItem.Path);

                if (!match)
                {
                    brokenQuery.AddBranchWithMappingItem(mappingItem, assemblies);
                }
            }

            return brokenQuery.SubBranches.Count > 0 ? brokenQuery : null;
        }

        private bool MapAssembliesToBranch(Branch branch, List<Assembly> assemblies, List<MappingPath> path)
        {

            if (branch.Path.SequenceEqual(path))
            {
                branch.SetAssemblies(assemblies);
                return true;
            }
            else
            {
                if (branch.SubBranches.Count == 0)
                {
                    return false;
                }
                foreach (var subBranch in branch.SubBranches)
                {
                    bool subMatch = MapAssembliesToBranch(subBranch, assemblies, path);
                    if (subMatch)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string SerializeMappingItems()
        {
            var mappingsJson = new StringBuilder();
            mappingsJson.Append($"[{string.Join(",", MappingItems.Select(t => t.Serialize()))}]");
            return mappingsJson.ToString();
        }

        public Mapping Copy(string newName)
        {
            return new Mapping()
            {
                Name = newName,
                Project = Project,
                MappingItems = MappingItems
            };
        }

        private static List<MappingItem> ExtractMappingItems(List<Branch> branches, string queryName)
        {
            var mappingItems = new List<MappingItem>();
            foreach (var branch in branches)
            {
                if (branch.Assemblies != null && branch.Assemblies.Count > 0)
                {
                    mappingItems.Add(new MappingItem()
                    {
                        AssemblyIds = branch.Assemblies.Select(b => b.Id).ToList(),
                        Path = branch.Path,
                        QueryName = queryName
                    });
                }
            }
            return mappingItems;
        }
    }
}
