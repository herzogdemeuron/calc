using Speckle.Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calc.Core.Color;

namespace Calc.Core.Objects
{
    public class Forest : IHasProject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("forest_name")]
        public string Name { get; set; }
        [JsonProperty("trees")] // for recieving the tree JSON from the API
        public List<Tree> Trees { get; set; }
        [JsonProperty("project_id")]
        public Project Project { get; set; }

        public void SetBranchColorsBy(string method)
        {
            List<Branch> branches = this.Trees.ConvertAll(tree => (Branch)tree);

            switch (method)
            {
                case "branches":
                    BranchPainter.ColorBranchesByBranch(branches);
                    break;
                case "buildups":
                    BranchPainter.ColorBranchesByBuildup(branches);
                    break;
                default:
                    break;
            }
        }

        public string SerializeTrees()
        {
            var treesJson = new StringBuilder();
            treesJson.Append($"[{string.Join(",", Trees.Select(t => t.Serialize()))}]");
            return treesJson.ToString();
        }
    }
}
