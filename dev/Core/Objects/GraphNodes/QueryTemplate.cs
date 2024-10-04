using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calc.Core.Color;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes
{
    public class QueryTemplate : IHasProject, IGraphNode
    {
        [JsonIgnore]
        public bool IsBlack { get; set; } = false; // if the query template is black ( for left over elements )
        [JsonIgnore]
        public List<CalcElement> Elements { get => GetElements(); }
        [JsonIgnore]
        public double TotalLength { get => SubBranches.Sum(s => s.TotalLength); }
        [JsonIgnore]
        public double TotalArea { get => SubBranches.Sum(s => s.TotalArea); }
        [JsonIgnore]
        public double TotalVolume { get => SubBranches.Sum(s => s.TotalVolume); }
        [JsonIgnore]
        public List<string> ElementIds => Elements.Select(e => e.Id).ToList();
        [JsonIgnore]
        public List<Branch> SubBranches => Queries.ConvertAll(query => (Branch)query);
        [JsonIgnore]
        public HslColor HslColor { get; set; } = ItemPainter.DefaultColor;
        [JsonProperty("id")]
        public int Id { get; set; } = -1;
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonIgnore]
        private List<Query> _queries;
        [JsonProperty("queries")] // for receiving the query JSON from the API
        public List<Query> Queries
        {
            get => _queries;
            set
            {
                _queries = value;
                if (_queries != null)
                {
                    foreach (var query in _queries)
                    {
                        query.QueryTemplate = this;
                    }
                }
            }
        }
        [JsonProperty("project_id")]
        public CalcProject Project { get; set; }

        public void SetBranchColorsBy(string method)
        {
            List<Branch> branches = Queries.ConvertAll(query => (Branch)query);

            switch (method)
            {
                case "branches":
                    ItemPainter.ColorBranchesByBranch(branches);
                    break;
                case "assemblies":
                    ItemPainter.ColorBranchesByAssembly(branches);
                    break;
                default:
                    break;
            }
        }

        public HashSet<string> GetAllParameters()
        {
            var parameters = new HashSet<string>();
            foreach (Query query in Queries)
            {
                parameters.UnionWith(query.FilterConfig.GetAllParameters());
                parameters.UnionWith(query.BranchConfig);
            }
            return parameters;
        }

        /// <summary>
        /// Plants all queries in the query template and returns the remaining elements
        /// </summary>
        public List<CalcElement> PlantQueries(List<CalcElement> searchElements)
        {
            foreach (Query query in Queries)
            {
                searchElements = query.Plant(searchElements);
            }
            return searchElements;
        }

        public string SerializeQueries()
        {
            var queriesJson = new StringBuilder();
            queriesJson.Append($"[{string.Join(",", Queries.Select(t => t.Serialize()))}]");
            return queriesJson.ToString();
        }

        private List<CalcElement> GetElements()
        {
            List<CalcElement> elements = new();

            foreach (Query query in Queries)
            {
                elements.AddRange(query.Elements);
            }

            return elements;
        }
    }
}
