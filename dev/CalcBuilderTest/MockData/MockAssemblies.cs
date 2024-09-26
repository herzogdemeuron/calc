using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CalcBuilderTest
{
    internal class MockAssemblies
    {
       
        public static List<Assembly> GetAssemblies()
        {
            return new List<Assembly>
            {
                new Assembly
                {
                    Id = 1,
                    Name = "AssemblyName1",
                    Standard = "Source1",
                    AssemblyUnit = Unit.m2,
                    Group = new AssemblyGroup {Name = "AssemblyGroup1" },
                    Description = "This is Assembly1 Description.",
                    AssemblyComponents = new ObservableCollection<AssemblyComponent>
                    {
                        new AssemblyComponent
                        {
                            MaterialComponentSet = new MaterialComponentSet()
                            {
                                MainMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[0], 1)
                            },
                            LayerComponent = new LayerComponent("targetTypeName0", "targetMaterialName0")

                        },
                          new AssemblyComponent
                        {
                               MaterialComponentSet = new MaterialComponentSet()
                            {
                                MainMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[1], 0.95),
                                SubMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[2], 0.05),
                            },                            
                            LayerComponent = new LayerComponent("targetTypeName1", "targetMaterialName1")
                        }
                    }
                },
                new Assembly
                {
                    Id = 2,
                    Name = "AssemblyName2",
                    Standard = "Source1",
                    AssemblyUnit = Unit.m2,
                    Group = new AssemblyGroup {Name = "AssemblyGroup2" },
                    Description = "This is Assembly2 Description.",
                    AssemblyComponents = new ObservableCollection<AssemblyComponent>
                    {
                        new AssemblyComponent
                        {
                             MaterialComponentSet = new MaterialComponentSet()
                             {
                                MainMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[1], 0.95),
                                SubMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[2], 0.05),                            
                             },                            
                            LayerComponent = new LayerComponent("targetTypeName1", "targetMaterialName1")
                        }
                    }
                },
            };
        }
    }
}
