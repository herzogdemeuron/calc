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
                        new BasicParameter("pieceParam1", Unit.piece, 6),
                        new BasicParameter("lengthParam1", Unit.m, 1.3),
                        new BasicParameter("areaParam1", Unit.m2, 3.3),
                        new BasicParameter("volumeParam1", Unit.m3, 4.1)
                    )
                );

            var layer2 = new LayerComponent
                (
                    "RevitMaterial2",
                    new BasicParameterSet
                    (
                        new BasicParameter("pieceParam2", Unit.piece, 2),
                        new BasicParameter("lengthParam2", Unit.m, 2.3),
                        new BasicParameter("areaParam2", Unit.m2, 3.6),
                        new BasicParameter("volumeParam2", Unit.m3, 7.1)
                        )
                    );
            var layer3 = new LayerComponent
                (
                    "RevitMaterial3",
                    new BasicParameterSet
                    (
                        new BasicParameter("pieceParam3", Unit.piece, 3),
                        new BasicParameter("lengthParam3", Unit.m, 3.3),
                        new BasicParameter("areaParam3", Unit.m2, 4.3),
                        new BasicParameter("volumeParam3", Unit.m3, 5.1)
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
