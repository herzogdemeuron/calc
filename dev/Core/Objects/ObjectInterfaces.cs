using Calc.Core.Color;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Objects
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        piece, m, m2, m3
    }

    public interface IHasProject
    {
        CalcProject Project { get; set; }
    }

    public interface IShowName
    {
        string ShowName { get; }
    }

    public interface IGraphNode
    {
        List<CalcElement> Elements { get; }
        List<string> ElementIds { get; }
        List<Branch> SubBranches { get; }
        HslColor HslColor { get; set; }
    }

    /// <summary>
    /// can be searched with group name and name
    /// </summary>
    public interface ISearchable
    {
        string Name { get; }
        string GroupName{ get; }
    }

    public interface IColorizable
    {
        string ColorIdentifier { get; }
        HslColor HslColor { get; set; }
    }

    public interface  ICalcComponent
    {
        double? Thickness { get; }
        string Name { get; }
        HslColor HslColor { get;}
        BasicParameterSet BasicParameterSet { get; set; }
        bool HasParamError { get; }
    }


}
