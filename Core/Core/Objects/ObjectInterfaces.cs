using Calc.Core.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core.Objects
{
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
