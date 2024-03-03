using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    public class BasicParameterSet
    {
        private BasicParameter lenParam;
        private BasicParameter areaParam;
        private BasicParameter volParam;
        private BasicParameter pieceParam;

        public BasicParameterSet(BasicParameter pieceParam, BasicParameter lenParam, BasicParameter areaParam, BasicParameter volParam)
        {
            this.lenParam = lenParam;
            this.areaParam = areaParam;
            this.volParam = volParam;
            this.pieceParam = pieceParam;
        }

        public BasicParameter GetQuantity(Unit unit)
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

    }
}
