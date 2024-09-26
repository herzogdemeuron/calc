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
                    Name = "BuildupName1",
                    Standard = "Source1",
                    BuildupUnit = Unit.m2,
                    Group = new AssemblyGroup {Name = "BuildupGroup1" },
                    Description = "This is Buildup1 Description.",
                    BuildupComponents = new ObservableCollection<AssemblyComponent>
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
                    Name = "BuildupName2",
                    Standard = "Source1",
                    BuildupUnit = Unit.m2,
                    Group = new AssemblyGroup {Name = "BuildupGroup2" },
                    Description = "This is Buildup2 Description.",
                    BuildupComponents = new ObservableCollection<AssemblyComponent>
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
