using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Calc.Core.Filtering;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Materials;
using Newtonsoft.Json.Linq;

namespace Calc.Tests.CoreTest;

public class MockMaterials
{
    public List<Material> Materials { get; set; } = new List<Material>();

    public MockMaterials()
    {
        Materials.Add(new Material
        {
            Id = 1,
            Name = "Material 1",
            GWP = 1.1m,
            GE = 1.2m,
            Cost = 1.3m,
            MaterialUnit = Unit.CubicMeter,
            Category = "Category 1",
            Standard = "Source 1",
            SourceCode = "Source Code 1"
        });
        Materials.Add(new Material
        {
            Id = 2,
            Name = "Material 2",
            GWP = 2.1m,
            GE = 2.2m,
            Cost = 2.3m,
            MaterialUnit = Unit.Kilogram,
            Category = "Category 2",
            Standard = "Source 2",
            SourceCode = "Source Code 2"
        });
        Materials.Add(new Material
        {
            Id = 3,
            Name = "Material 3",
            GWP = 3.1m,
            GE = 3.2m,
            Cost = 3.3m,
            MaterialUnit = Unit.SquareMeter,
            Category = "Category 3",
            Standard = "Source 3",
            SourceCode = "Source Code 3"
        });
    }


}
