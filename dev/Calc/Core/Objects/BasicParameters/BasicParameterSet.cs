using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    public class BasicParameterSet
    {
        private BasicParameter pieceParam;
        private BasicParameter lenParam;
        private BasicParameter areaParam;
        private BasicParameter volParam;

        public BasicParameterSet(BasicParameter pieceParam, BasicParameter lenParam, BasicParameter areaParam, BasicParameter volParam)
        {
            this.pieceParam = pieceParam;
            this.lenParam = lenParam;
            this.areaParam = areaParam;
            this.volParam = volParam;
        }

        public BasicParameter GetAmountParam(Unit unit)
        {
            switch (unit)
            {
                case Unit.m:
                    return lenParam;
                case Unit.m2:
                    return areaParam;
                case Unit.m3:
                    return volParam;
                case Unit.piece:
                    return pieceParam;
                default:
                    throw new Exception($"Unit not recognized: {unit}");
            }
        }

        public void Add(BasicParameterSet other)
        {
            pieceParam = pieceParam.PerformOperation(Operation.Add, other.pieceParam);
            lenParam = lenParam.PerformOperation(Operation.Add, other.lenParam);
            areaParam = areaParam.PerformOperation(Operation.Add, other.areaParam);
            volParam = volParam.PerformOperation(Operation.Add, other.volParam);
        }

        public void Set(BasicParameter param)
        {
            var unit = param.Unit;
            switch (unit)
            {
                case Unit.m:
                    lenParam = param;
                    break;
                case Unit.m2:
                    areaParam = param;
                    break;
                case Unit.m3:
                    volParam = param;
                    break;
                case Unit.piece:
                    pieceParam = param;
                    break;
                default:
                    throw new Exception($"Unit not recognized: {unit}");
            }
        }

        public static BasicParameterSet ErrorParamSet()
        {
            return new BasicParameterSet
                    (
                    BasicParameter.ErrorParam(Unit.piece),
                    BasicParameter.ErrorParam(Unit.m),
                    BasicParameter.ErrorParam(Unit.m2),
                    BasicParameter.ErrorParam(Unit.m3)
                    );
        }
    }
}
