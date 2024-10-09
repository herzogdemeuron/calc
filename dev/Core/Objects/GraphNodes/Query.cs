using System.Text;
using System.Linq;
using System.Collections.Generic;
using Calc.Core.Filtering;
using Newtonsoft.Json;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes
{
    /// <summary>
    /// Used by calc project.
    /// A query is both the query schema and the query result.
    /// A query is the top level object of the branches.
    /// </summary>
    public class Query : Branch, IGraphNode
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("filter_config")]
        public GroupCondition FilterConfig { get; set; }
        [JsonProperty("branch_config")]
        public List<string> BranchConfig { get; set; } // List of parameters to group by

        /// <summary>
        /// Performs a query on a list of elements.Filters the elements and group them.
        /// </summary>
        /// <returns>A new list of remaining elements</returns>
        internal List<CalcElement> Perform(List<CalcElement> searchElements)
        {
            this.Query = this;
            Elements = FilterElements(searchElements, FilterConfig);
            SubBranches.Clear();
            if (BranchConfig != null && BranchConfig.Count > 0)
            {
                CreateBranches(BranchConfig);
            }
            List<CalcElement> remain = searchElements.Where(e => !Elements.Contains(e)).ToList();
            return remain;
        }

        /// <summary>
        /// Creates a new branch from the mappingitem and adds assemblies to it.
        /// </summary>
        internal void AddBranchWithMappingItem(MappingItem mappingItem, List<Assembly> assemblies)
        {
            var paths = mappingItem.Path;
            Branch currentBranch = this;
            foreach (var path in paths)
            {
                var parameter = path.Parameter;
                var value = path.Value;
                currentBranch = currentBranch.AddBranch(parameter, value, assemblies);
            }
        }

        /// <summary>
        /// Filters a list of elements based on a group condition.
        /// </summary>
        private List<CalcElement> FilterElements(List<CalcElement> elements, GroupCondition filter)
        {
            List<CalcElement> filteredElements = new List<CalcElement>();

            foreach (CalcElement element in elements)
            {
                if (filter.Evaluate(element))
                {
                    filteredElements.Add(element);
                }
            }
            return filteredElements;
        }

        /// <summary>
        /// Creates a query for a category.
        /// </summary>
        public static Query CreateCategoryQuery(string categoryName)
        {
            Query query = new Query();
            query.Name = categoryName;

            SimpleCondition condition = new SimpleCondition()
            {
                Method = "equals",
                Parameter = "Category",
                Value = categoryName
            };
            ConditionContainer conditionContainer = new ConditionContainer()
            {
                Type = "simple_condition",
                Condition = condition
            };
            query.FilterConfig = new GroupCondition()
            {
                Conditions = new List<ConditionContainer>() { conditionContainer },
                Operator = "and"
            };
            query.BranchConfig = new List<string>() { "type:Type Name" };

            return query;
        }

        public string Serialize()
        {
            var json = new StringBuilder();
            json.Append("{");
            json.Append($"\"Name\": \"{Name}\",");
            json.Append($"\"FilterConfig\": {JsonConvert.SerializeObject(FilterConfig)},");
            json.Append($"\"BranchConfig\": [{string.Join(",", BranchConfig.Select(b => $"\"{b}\""))}]");
            json.Append("}");
            return json.ToString();
        }
    }
}
