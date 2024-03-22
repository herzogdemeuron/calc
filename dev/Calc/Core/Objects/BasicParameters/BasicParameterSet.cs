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

        public void AddPiece()
        {
            pieceParam = pieceParam.PerformOperation(Operation.Add, 1);
        }

    }
}
