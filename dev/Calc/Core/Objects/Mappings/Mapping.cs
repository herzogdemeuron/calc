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

        public Mapping(string mappingName, params Forest[] forests)
        {
            MappingItems = new List<MappingItem>();
            foreach (var forest in forests)
            {
                if(forest == null)
                {
                    continue;
                }

                foreach (var tree in forest.Trees)
                {
                    MappingItems.AddRange(ExtractMappingItems(tree.Flatten(), tree.Name));
                }
            }
            Name = mappingName;
        }

        public void AddMappingItems(List<MappingItem> mappingItems)
        {
            MappingItems.AddRange(mappingItems);
        }

        /// <summary>
        /// Assigns the assemblies to the tree based on the mapping.
        /// Automatically determines which mapping to use based on the tree name.
        /// Returns a broken tree if the mapping path is not found.
        /// </summary>
        public Tree ApplyToTree(Tree tree, List<Assembly> allAssemblies)
        {
            var brokenTree = new Tree()
            {
                Name = tree.Name,
                ParentTree = tree
            };

            tree.ResetAssemblies();
            // find the mapping items that apply to this tree
            var mappingItems = MappingItems.Where(mappingItem => mappingItem.TreeName == tree.Name);
            if (!mappingItems.Any())
            {
                return null;
            }

            foreach (var mappingItem in mappingItems)
            {
                var assemblyIds = mappingItem.BuildupIds.Take(2).ToList();
                var assemblies = allAssemblies.Where(b => assemblyIds.Contains(b.Id)).ToList();
                var match = MapAssembliesToBranch(tree, assemblies, mappingItem.Path);

                if (!match)
                {
                    brokenTree.AddBranchWithMappingItem(mappingItem, assemblies);
                }
            }

            return brokenTree.SubBranches.Count > 0 ? brokenTree : null;
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

        private static List<MappingItem> ExtractMappingItems(List<Branch> branches, string treeName)
        {
            var mappingItems = new List<MappingItem>();
            foreach (var branch in branches)
            {
                if (branch.Assemblies != null && branch.Assemblies.Count > 0)
                {
                    mappingItems.Add(new MappingItem()
                    {
                        BuildupIds = branch.Assemblies.Select(b => b.Id).ToList(),
                        Path = branch.Path,
                        TreeName = treeName
                    });
                }
            }
            return mappingItems;
        }
    }
}
