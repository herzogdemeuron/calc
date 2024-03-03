using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcBuilderTest.MockData
{
    internal class MockLayerComponents
    {
        public List<LayerComponent> GetLayerComponents()
        {
            var result = new List<LayerComponent>()
            {
                new LayerComponent("targetTypeName0", "targetMaterialName0")
                {
                    Ids = new List<int> { 155,144,188 },
                    basicParameterSet = new BasicParameterSet(
                        new BasicParameter(1, Unit.piece),
                        new BasicParameter(1, Unit.m),
                        new BasicParameter(1, Unit.m2),
                        new BasicParameter(1, Unit.m3))

                                                                                                                                       )

            };


            return result;
        }
    }
}
