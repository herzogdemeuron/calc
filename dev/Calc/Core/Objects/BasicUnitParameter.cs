using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects
{
    public class BasicUnitParameter
    {
        public string Name { get; set; }
        public Unit Unit { get; set; }
        public ParameterErrorType? ErrorType { get; set; }

        private decimal? _value;

        public decimal? Value
        {
            get
            {
                return _value;
            }
            set
            {

                if (value != null)
                {
                    _value = Math.Round((decimal)value, 3);
                }
                else
                {
                    _value = null;
                }

            }
        }
    }
}
