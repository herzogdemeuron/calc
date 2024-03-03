using Calc.Core.Objects.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcBuilderTest
{
    internal class MockMaterials
    {
        public static List<Material> GetMaterials()
        {
            return new List<Material>
            {
                new Material
                {
                    Id = "001",
                    Name = "MaterialName1",
                    Category = "Category 1",
                    Source = "Source1",
                    SourceCode = "SourceCode1",
                    ValidUntil = 2024,
                    ValidFrom = 2020,
                    BulkDensity = 140,
                    GWP = 1.5,
                    GE = 2.5
                },
                new Material
                {
                    Id = "002",
                    Name = "MaterialName2",
                    Category = "Category 1",
                    Source = "Source1",
                    SourceCode = "SourceCode1",
                    ValidUntil = 2025,
                    ValidFrom = 2021,
                    Grammage = 150,
                    GWP = 1.6,
                    GE = 2.6
                },
                new Material
                {
                    Id = "003",
                    Name = "MaterialName3",
                    Category = "Category 2",
                    Source = "Source1",
                    SourceCode = "SourceCode1",
                    ValidUntil = 2026,
                    ValidFrom = 2022,
                    LinearDensity = 160,
                    GWP = 1.7,
                    GE = 2.7
                },
                new Material
                {
                    Id = "004",
                    Name = "MaterialName4",
                    Category = "Category 2",
                    Source = "Source1",
                    SourceCode = "SourceCode1",
                    ValidUntil = 2027,
                    ValidFrom = 2023,
                    BulkDensity = 190,
                    LinearDensity = 170,
                    GWP = 1.8,
                    GE = 2.8
                }
            };
        }
    }
}
