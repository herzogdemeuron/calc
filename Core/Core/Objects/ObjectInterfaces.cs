using Calc.Core.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core.Objects
{
    public interface IFilter
    {
        List<CalcElement> Elements { get; }
        List<string> ElementIds { get; }
        string Parameter { get; set; }
        string Method { get; set; }
        string Value { get; set; }
    }

    public interface IHasProject
    {
        Project Project { get; set; }
    }

    public interface IGraphNode
    {
        List<CalcElement> Elements { get; }
        List<Branch> SubBranches { get; }
        HslColor HslColor { get; set; }

    }
}
