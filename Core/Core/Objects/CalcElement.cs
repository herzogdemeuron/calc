using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Color;
using System;
using Speckle.Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Calc.Core.Objects
{
    public struct CalcElement
    {
        public string Id;
        public string TypeName;
        public Dictionary<string, object> Fields;
        private Dictionary<Unit, decimal?> _quantities;

        public CalcElement(string id,
            string type,
            Dictionary<string, object> fields,
            decimal? length,
            decimal? area,
            decimal? volume
            )
        {
            this.Id = id;
            this.TypeName = type;
            this.Fields = fields;
            this._quantities = new Dictionary<Unit, decimal?>
            {
                { Unit.each, 1 },
                { Unit.m, length },
                { Unit.m2, area },
                { Unit.m3, volume }
            };
        }

        public decimal? GetQuantityByUnit(Unit unit, int roundDigits = 3)
        {
            if (_quantities.TryGetValue(unit, out decimal? value))
            {
                return value == null ? null : Math.Round((decimal)value, roundDigits);
            }
            else
            {
                throw new ArgumentException($"Unit not recognized: {unit}");
            }

        }
    }


}
