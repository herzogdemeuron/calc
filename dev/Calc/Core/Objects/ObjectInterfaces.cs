using Calc.Core.Color;
using Speckle.Newtonsoft.Json.Converters;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.BasicParameters;

namespace Calc.Core.Objects
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        piece, m, m2, m3
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialFunction
    {
        Structural, Facade, InteriorFinishes
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
        string Title { get; }
        BasicParameterSet BasicParameterSet { get; set; }
    }


}
