using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Color;
using System;
using Speckle.Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Calc.Core.Objects.BasicParameters;

namespace Calc.Core.Objects
{
    public struct CalcElement
    {
        public string Id;
        public string TypeName;
        public Dictionary<string, object> Fields;
        private Dictionary<Unit, BasicParameter> _quantities;

        public CalcElement
            (
                string id,
                string type,
                Dictionary<string, object> fields,
                BasicParameter lenParam,
                BasicParameter areaParam,
                BasicParameter volParam
             )
            {
            this.Id = id;
            this.TypeName = type;
            this.Fields = fields;
            this._quantities 
                = new Dictionary<Unit, BasicParameter>
                    {
                        { Unit.piece, new BasicParameter() { Amount = 1, Unit = Unit.piece } },
                        { Unit.m, lenParam },
                        { Unit.m2, areaParam },
                        { Unit.m3, volParam }
                    };
            }

        public readonly BasicParameter GetBasicUnitParameter(Unit unit)
        {
            if (!_quantities.ContainsKey(unit))
            {
                throw new Exception($"Unit not recognized: {unit}");
            }
            return _quantities[unit];
        }
    }


}
