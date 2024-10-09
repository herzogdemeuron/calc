using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calc.Core.Color;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes
{
    /// <summary>
    /// Used by calc project.
    /// It is both a set of queries and the result of the queries.
    /// It is also used to summarize the leftover queries (for elements that are not fallen into any query results)
    /// and the broken query set (for broken mappings that never exists agin in the current query result structure).
    /// </summary>
    public class QueryTemplate : IHasProject, IGraphNode
    {
        [JsonIgnore]
        public bool IsLeftover { get; set; } = false;
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
        [JsonProperty("queries")]
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

        /// <summary>
        /// Sets the colors of the branches based on the coloring method (branches, assemblies).
        /// </summary>
        /// <param name="method"></param>
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

        /// <summary>
        /// Gets all parameters that are going to be used in the query template,
        /// this is used to simplify the element creation, only getting the nescessary parameters.
        /// </summary>
        public List<string> GetAllParameters()
        {
            var parameters = new HashSet<string>();
            foreach (Query query in Queries)
            {
                parameters.UnionWith(query.FilterConfig.GetAllParameters());
                parameters.UnionWith(query.BranchConfig);
            }
            return parameters.ToList();
        }

        /// <summary>
        /// Performs all queries in the query template and returns the remaining elements
        /// </summary>
        public List<CalcElement> PerformQueries(List<CalcElement> searchElements)
        {
            foreach (Query query in Queries)
            {
                searchElements = query.Perform(searchElements);
            }
            return searchElements;
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
