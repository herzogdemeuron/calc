using Calc.Core.Color;
using Speckle.Newtonsoft.Json.Converters;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.Objects.GraphNodes;

namespace Calc.Core.Objects
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        each, m, m2, m3
    }

    public interface IHasProject
    {
        Project Project { get; set; }
    }

    public interface IGraphNode
    {
        List<CalcElement> Elements { get; }
        List<string> ElementIds { get; }
        List<Branch> SubBranches { get; }
        HslColor HslColor { get; set; }

    }
}
