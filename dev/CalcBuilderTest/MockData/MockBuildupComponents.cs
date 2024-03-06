using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Results;
using System.Collections.Generic;

namespace CalcBuilderTest.MockData
{
    internal class MockBuildupComponents
    {
        public static List<BuildupComponent> GetBuildupComponents()
        {
            var result = new List<BuildupComponent>()
            {
                new BuildupComponent
            }
        }

        public static List<LayerComponent> GetLayerComponentsFromApp()
        {
            var result = new List<LayerComponent>()
            {
                new LayerComponent
                (
                "RevitMaterialName1",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 3 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                 new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 1 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                  new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 2 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, Value = 11 },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),

            };
            return result;
            public static List<LayerComponent> GetLayerComponentsFromRevit()
        {
            var result = new List<LayerComponent>()
            {
                new LayerComponent
                (
                "RevitMaterialName1",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 3 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                 new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 1 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                  new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 2 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, Value = 11 },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),

            };
            return result;
        }
    }
}
