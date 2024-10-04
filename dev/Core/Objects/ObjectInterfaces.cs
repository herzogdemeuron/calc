using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Calc.Core.Objects
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        piece, m, m2, m3
    }

    /// <summary>
    /// Implement this to get project related mappings and query templates from directus.
    /// </summary>
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
    /// Can be searched with group name and name.
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
