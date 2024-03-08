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
                    Id = 1,
                    Name = "MaterialName1",
                    Standard = "Source1",
                    Category = "Category 1",
                    Thickness = 0.2,
                    ValidUntil = 2024,
                    ValidFrom = 2020,
                    Density = 140,
                    MaterialUnit = Calc.Core.Objects.Unit.m3,
                    GWP = 1.5,
                    GE = 2.5
                },
                new Material
                {
                    Id = 2,
                    Name = "MaterialName2",
                    Category = "Category 1",
                    Standard = "Source1",
                    Thickness = 0.3,
                    ValidUntil = 2025,
                    ValidFrom = 2021,
                    Density = 150,
                    MaterialUnit = Calc.Core.Objects.Unit.m2,
                    GWP = 1.6,
                    GE = 2.6
                },
                new Material
                {
                    Id = 3,
                    Name = "MaterialName3",
                    Category = "Category 2",
                    Standard = "Source1",
                    Thickness = 0.4,
                    ValidUntil = 2026,
                    ValidFrom = 2022,
                    Density = 160,
                    MaterialUnit = Calc.Core.Objects.Unit.m,
                    GWP = 1.7,
                    GE = 2.7
                },
                new Material
                {
                    Id = 4,
                    Name = "MaterialName4",
                    Category = "Category 2",
                    Standard = "Source1",
                    Thickness = 0.5,
                    ValidUntil = 2027,
                    ValidFrom = 2023,
                    Density = 190,
                    MaterialUnit = Calc.Core.Objects.Unit.piece,
                    GWP = 1.8,
                    GE = 2.8
                }
            };
        }
    }
}
