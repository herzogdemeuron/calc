using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ui_test
{
    public class MockBuildupComponentCreator : IBuildupComponentCreator
    {
        public List<BuildupComponent> CreateBuildupComponentsFromSelection()
        {
            var layer1 = new LayerComponent
                (
                    "RevitMaterial1",
                    new BasicParameterSet
                    ( 
                        new BasicParameter() { Name="pieceParam1", Unit=Unit.piece, Amount=6 },
                        new BasicParameter() { Name = "lengthParam1", Unit = Unit.m, Amount = 1.3 },
                        new BasicParameter() { Name = "areaParam1", Unit = Unit.m2, Amount = 3.3 },
                        new BasicParameter() { Name = "volumeParam1", Unit = Unit.m3, Amount = 4.1 }
                    )
                );

            var layer2 = new LayerComponent
                (
                    "RevitMaterial2",
                    new BasicParameterSet
                    (
                        new BasicParameter() { Name = "pieceParam2", Unit = Unit.piece, Amount = 2 },
                        new BasicParameter() { Name = "lengthParam2", Unit = Unit.m, Amount = 2.3 },
                        new BasicParameter() { Name = "areaParam2", Unit = Unit.m2, Amount = 3.6 },
                        new BasicParameter() { Name = "volumeParam2", Unit = Unit.m3, Amount = 7.1 }
                        )
                    );
            var layer3 = new LayerComponent
                (
                    "RevitMaterial3",
                    new BasicParameterSet
                    (
                        new BasicParameter() { Name = "pieceParam3", Unit = Unit.piece, Amount = 1 },
                        new BasicParameter() { Name = "lengthParam3", Unit = Unit.m, Amount = 3.3 },
                        new BasicParameter() { Name = "areaParam3", Unit = Unit.m2, Amount = 4.3 },
                        new BasicParameter() { Name = "volumeParam3", Unit = Unit.m3, Amount = 5.1 }
                        )
                    );


            var bc1 = new BuildupComponent()
            {
                Name = "BuildupComponent1",
                ElementIds = new List<int> { 101, 102, 103 },
                TypeIdentifier = 201,
                IsCompoundElement = true,
                Thickness = 0.5,
                LayerComponents = new List<LayerComponent> { layer1, layer2},
                IsNormalizer = true
            };

            var bc2 = new BuildupComponent()
            {
                Name = "BuildupComponent2",
                ElementIds = new List<int> { 104, 106 },
                TypeIdentifier = 202,
                IsCompoundElement = false,
                LayerComponents = new List<LayerComponent> { layer3},
                IsNormalizer = false
            };

            return new List<BuildupComponent> { bc1, bc2 };
        }

    }
}
