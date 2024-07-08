using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calc.Core.Color;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes
{
    public class Forest : IHasProject, IGraphNode
    {
        [JsonIgnore]
        public bool IsDark { get; set; } = false; // if the forest is a dark forest ( for left over elements )
        [JsonIgnore]
        public List<CalcElement> Elements { get => GetElements(); }
        [JsonIgnore]
        public double TotalArea { get => SubBranches.Sum(s => s.TotalArea); }
        [JsonIgnore]
        public double TotalVolume { get => SubBranches.Sum(s => s.TotalVolume); }
        [JsonIgnore]
        public List<string> ElementIds => Elements.Select(e => e.Id).ToList();
        [JsonIgnore]
        public List<Branch> SubBranches => Trees.ConvertAll(tree => (Branch)tree);
        [JsonIgnore]
        public HslColor HslColor { get; set; } = ItemPainter.DefaultColor;
        [JsonProperty("id")]
        public int Id { get; set; } = -1;
        [JsonProperty("forest_name")]
        public string Name { get; set; }
        [JsonIgnore]
        private List<Tree> _trees;
        [JsonProperty("trees")] // for receiving the tree JSON from the API
        public List<Tree> Trees
        {
            get => _trees;
            set
            {
                _trees = value;
                if (_trees != null)
                {
                    foreach (var tree in _trees)
                    {
                        tree.ParentForest = this;
                    }
                }
            }
        }
        [JsonProperty("project_id")]
        public Project Project { get; set; }

        public void SetBranchColorsBy(string method)
        {
            List<Branch> branches = Trees.ConvertAll(tree => (Branch)tree);

            switch (method)
            {
                case "branches":
                    ItemPainter.ColorBranchesByBranch(branches);
                    break;
                case "buildups":
                    ItemPainter.ColorBranchesByBuildup(branches);
                    break;
                default:
                    break;
            }
        }

        public HashSet<string> GetAllParameters()
        {
            var parameters = new HashSet<string>();
            foreach (Tree tree in Trees)
            {
                parameters.UnionWith(tree.FilterConfig.GetAllParameters());
                parameters.UnionWith(tree.BranchConfig);
            }
            return parameters;
        }

        /// <summary>
        /// Plants all trees in the forest and returns the remaining elements
        /// </summary>
        public List<CalcElement> PlantTrees(List<CalcElement> searchElements)
        {
            foreach (Tree tree in Trees)
            {
                searchElements = tree.Plant(searchElements);
            }
            return searchElements;
        }

        public string SerializeTrees()
        {
            var treesJson = new StringBuilder();
            treesJson.Append($"[{string.Join(",", Trees.Select(t => t.Serialize()))}]");
            return treesJson.ToString();
        }

        private List<CalcElement> GetElements()
        {
            List<CalcElement> elements = new();

            foreach (Tree tree in Trees)
            {
                elements.AddRange(tree.Elements);
            }

            return elements;
        }
    }
}
