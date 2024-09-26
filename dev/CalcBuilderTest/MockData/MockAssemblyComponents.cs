﻿using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Assemblies;
using System.Collections.Generic;

namespace CalcBuilderTest.MockData
{
    internal class MockAssemblyComponents
    {
        public static List<AssemblyComponent> GetAssemblyComponents()
        {
            var result = new List<AssemblyComponent>()
            {
                new AssemblyComponent
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
                        new BasicParameter() {Unit = Unit.piece, Amount = 3 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
                ),
                 new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Amount = 1 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
                ),
                  new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Amount = 2 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, Amount = 11 },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
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
                        new BasicParameter() {Unit = Unit.piece, Amount = 3 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
                ),
                 new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Amount = 1 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
                ),
                  new LayerComponent
                (
                "RevitMaterialName2",
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Amount = 2 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, Amount = 11 },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Amount = 1 })
                ),

            };
            return result;
        }
    }
}
