using System.Text;
using System.Linq;
using System.Collections.Generic;
using Calc.Core.Filtering;
using Newtonsoft.Json;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects.GraphNodes;

public class Tree : Branch, IGraphNode
{
    public string Name { get; set; }
    public GroupCondition FilterConfig { get; set; }
    private List<string> _branchConfig;
    public List<string> BranchConfig // List of parameters to group by
    {
        get { return _branchConfig; }
        set { _branchConfig = value.ToList(); }
    }

    /// <summary>
    /// Creates a tree from a list of elements and a list of parameters to group by
    /// </summary>
    public List<CalcElement> Plant(List<CalcElement> searchElements)
    {
        Elements = new ElementFilter().FilterElements(searchElements, FilterConfig);
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
    /// creates a new branch from the mappingitem and adds buildups to it
    /// </summary>
    public void AddBranchWithMappingItem(MappingItem mappingItem, List<Buildup> buildups)
    {
        var paths = mappingItem.Path;
        Branch currentBranch = this;
        foreach (var path in paths)
        {
            var parameter = path.Parameter;
            var value = path.Value;
            currentBranch = currentBranch.AddBranch(parameter, value, buildups);
        }
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
