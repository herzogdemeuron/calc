using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcBuilderTest.MockData
{
    internal class MockLayerComponents
    {
        public static List<LayerComponent> GetLayerComponents()
        {
            var result = new List<LayerComponent>()
            {
                new LayerComponent
                (
                    "RevitTypeName1", 
                "RevitMaterialName1",
                new List<int> { 155,144,188 },
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 3 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                 new LayerComponent
                (
                    "RevitTypeName1",
                "RevitMaterialName2",
                new List<int> { 45 },
                new BasicParameterSet(
                        new BasicParameter() {Unit = Unit.piece, Value = 1 },
                        new BasicParameter() {Name = "LenParamName", Unit = Unit.m, ErrorType = ParameterErrorType.ZeroValue},
                        new BasicParameter() {Name = "AreaParamName",Unit = Unit.m2, ErrorType = ParameterErrorType.ZeroValue },
                        new BasicParameter() {Name = "VolParamName",Unit = Unit.m3, Value = 1 })
                ),
                  new LayerComponent
                (
                    "RevitTypeName2",
                "RevitMaterialName2",
                new List<int> { 112,23 },
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
