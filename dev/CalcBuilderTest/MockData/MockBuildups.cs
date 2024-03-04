using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CalcBuilderTest
{
    internal class MockBuildups
    {
       
        public static List<Buildup> GetBuildups()
        {
            return new List<Buildup>
            {
                new Buildup
                {
                    Id = 1,
                    Name = "BuildupName1",
                    Source = "Source1",
                    BuildupUnit = Unit.m2,
                    Group = new BuildupGroup {Name = "BuildupGroup1" },
                    Description = "This is Buildup1 Description.",
                    Components = new ObservableCollection<BuildupComponent>
                    {
                        new BuildupComponent
                        {
                            MaterialComponentSet = new MaterialComponentSet()
                            {
                                MainMaterialComponent = new MaterialComponent(MockMaterials.GetMaterials()[0], 1)
                            },
                            LayerComponent = new LayerComponent("targetTypeName0", "targetMaterialName0")

                        },
                          new BuildupComponent
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
                new Buildup
                {
                    Id = 2,
                    Name = "BuildupName2",
                    Source = "Source1",
                    BuildupUnit = Unit.m2,
                    Group = new BuildupGroup {Name = "BuildupGroup2" },
                    Description = "This is Buildup2 Description.",
                    Components = new ObservableCollection<BuildupComponent>
                    {
                        new BuildupComponent
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
