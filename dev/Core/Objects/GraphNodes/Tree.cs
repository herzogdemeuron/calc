using System.Text;
using System.Linq;
using System.Collections.Generic;
using Calc.Core.Filtering;
using Newtonsoft.Json;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes;

public class Tree : Branch, IGraphNode
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("filter_config")]
    public GroupCondition FilterConfig { get; set; }
    private List<string> _branchConfig;  // needed?
    [JsonProperty("branch_config")]
    public List<string> BranchConfig // List of parameters to group by
    {
        get => _branchConfig;
        set { _branchConfig = value.ToList(); }
    }

    /// <summary>
    /// Creates a tree from a list of elements and a list of parameters to group by
    /// </summary>
    public List<CalcElement> Plant(List<CalcElement> searchElements)
    {
        Elements = FilterElements(searchElements, FilterConfig);
        SubBranches.Clear();
        this.ParentTree = this;

        if (BranchConfig != null && BranchConfig.Count > 0)
        {
            CreateBranches(BranchConfig);
        }

        searchElements.RemoveAll(e => Elements.Contains(e));
        return searchElements;
    }

    /// <summary>
    /// creates a new branch from the mappingitem and adds assemblies to it
    /// </summary>
    public void AddBranchWithMappingItem(MappingItem mappingItem, List<Assembly> assemblies)
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
    /// Makes a tree for a category.
    /// </summary>
    public static Tree MakeCategoryTree(string categoryName)
    {
        Tree tree = new Tree();
        tree.Name = categoryName;

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
        tree.FilterConfig = new GroupCondition()
        {
            Conditions = new List<ConditionContainer>() { conditionContainer },
            Operator = "and"
        };
        tree.BranchConfig = new List<string>() { "type:Type Name" };

        return tree;
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
