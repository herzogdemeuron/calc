using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core.Objects
{
    public class Mapping : IHasProject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("mapping_name")]
        public string Name { get; set; }
        [JsonProperty("project_id")]
        public Project Project { get; set; }
        [JsonProperty("mappings")]
        public List<MappingItem> MappingItems { get; set; }

        public Mapping()
        {
            // empty constructor for deserialization
        }

        public Mapping(Forest forest, string mappingName)
        {
            MappingItems = new List<MappingItem>();
            foreach (var tree in forest.Trees)
            {
                MappingItems.AddRange(ExtractMappingItems(tree.Flatten(), tree.Name));
            }
            Name = mappingName;
        }

        /// <summary>
        /// Assigns the buildups to the tree based on the mapping.
        /// Automatically determines which mapping to use based on the tree name.
        /// Example Use:
        /// <code>
        /// var buildups = await BuildupStorageDriver.GetAllBuildups();
        /// var mappings = await MappingStorageDriver.GetMappings("my mapping name");
        /// foreach (var tree in trees)
        /// {
        ///     MappingStorageDriver.ApplyMappingToTree(tree, mappings.First(), buildups);
        /// }
        /// </code>
        /// </summary>
        /// <param name="tree">The tree to assign buildups to.</param>
        /// <param name="buildups">The full list of buildups from the database.</param>
        /// <returns></returns>
        public Tree ApplyMappingToTree(Tree tree, List<Buildup> buildups)
        {
            tree.ResetBuildups();
            // search through the list of mappings for the mapping with the same tree name
            var mappingItems = MappingItems.Where(mappingItem => mappingItem.TreeName == tree.Name);
            if (!mappingItems.Any())
            {
                return tree;
            }

            // search through the branches of the tree to find the branch with the same parameter and value as the mapping
            // if the branch is found, get the buildup by id and apply the buildup to the branch
            foreach (var mappingItem in mappingItems)
            {
                var buildup = buildups.SingleOrDefault(b => b.Id == mappingItem.BuildupId);
                if (buildup == null)
                {
                    continue;
                }
                tree.MatchMapping(mappingItem.Parameter, mappingItem.Value, buildup);
            }
            return tree;
        }

        public string SerializeMappingItems()
        {
            var mappingsJson = new StringBuilder();
            mappingsJson.Append($"[{string.Join(",", MappingItems.Select(t => t.Serialize()))}]");
            return mappingsJson.ToString();
        }

        private static List<MappingItem> ExtractMappingItems(List<Branch> branches, string treeName)
        {
            var mappingItems = new List<MappingItem>();
            foreach (var branch in branches)
            {
                if (branch.Buildup != null)
                {
                    mappingItems.Add(new MappingItem()
                    {
                        BuildupId = branch.Buildup.Id,
                        Parameter = branch.Parameter,
                        Value = branch.Value,
                        TreeName = treeName
                    });
                }
            }
            return mappingItems;
        }
    }

    public class MappingItem
    {
        [JsonProperty("buildup_id")]
        public int BuildupId { get; set; }
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("tree_name")]
        public string TreeName { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
