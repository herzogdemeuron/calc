using Calc.Core.Objects.BasicParameters;
using System;
using System.Collections.Generic;

namespace Calc.Core.Objects.Elements
{
    /// <summary>
    /// A building element in calc.
    /// </summary>
    public struct CalcElement
    {
        public string Id;
        public string TypeName;
        public string Category;
        public Dictionary<string, object> Fields;
        private Dictionary<Unit, BasicParameter> _quantities;

        public CalcElement
            (
                string id,
                string category,
                string type,
                Dictionary<string, object> fields,
                BasicParameter lenParam,
                BasicParameter areaParam,
                BasicParameter volParam
             )
        {
            Id = id;
            Category = category;
            TypeName = type;
            Fields = fields;
            _quantities
                = new Dictionary<Unit, BasicParameter>
                    {
                        { Unit.piece, new BasicParameter() { Amount = 1, Unit = Unit.piece } },
                        { Unit.m, lenParam },
                        { Unit.m2, areaParam },
                        { Unit.m3, volParam }
                    };
        }

        internal readonly BasicParameter GetBasicUnitParameter(Unit unit)
        {
            if (!_quantities.ContainsKey(unit))
            {
                throw new Exception($"Unit not recognized: {unit}");
            }
            return _quantities[unit];
        }
    }


}
