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
}
