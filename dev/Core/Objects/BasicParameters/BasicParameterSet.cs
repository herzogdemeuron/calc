using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    /// <summary>
    /// Basic parameter set of all basic quantity units.
    /// </summary>
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

        /// <summary>
        /// Given a unit, gets the corresponding basic parameter.
        /// </summary>
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

        /// <summary>
        /// Adds a given basic parameter set to this one.
        /// </summary>
        public void Add(BasicParameterSet other)
        {
            pieceParam = pieceParam.PerformOperation(Operation.Add, other.pieceParam);
            lenParam = lenParam.PerformOperation(Operation.Add, other.lenParam);
            areaParam = areaParam.PerformOperation(Operation.Add, other.areaParam);
            volParam = volParam.PerformOperation(Operation.Add, other.volParam);
        }

        /// <summary>
        /// Forces set a basic parameter with a given one.
        /// </summary>
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

        /// <summary>
        /// Creates an error parameter set with all units set to error.
        /// </summary>
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
