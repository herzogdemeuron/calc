using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    public class BasicParameter
    {
        public string Name { get; set; }
        public Unit Unit { get; set; }
        public ParameterErrorType? ErrorType { get; set; }

        private double? _value;

        public double? Value
        {
            get
            {
                return _value;
            }
            set
            {

                if (value != null)
                {
                    _value = Math.Round((double)value, 3);
                }
                else
                {
                    _value = null;
                }

            }
        }
    }
}
