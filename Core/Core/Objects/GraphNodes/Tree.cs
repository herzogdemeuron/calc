using System.Text;
using System.Linq;
using System.Collections.Generic;
using Calc.Core.Filtering;
using Speckle.Newtonsoft.Json;

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

        // clear subbranches
        SubBranches = new List<Branch>();

        if (BranchConfig != null && BranchConfig.Count > 0)
        {
            CreateBranches(BranchConfig);
        }

        // remove this.Elements from searchElements using RemoveAll method
        searchElements.RemoveAll(e => Elements.Contains(e));

        return searchElements;
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
