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

        private double? _amount;

        public double? Amount
        {
            get
            {
                return _amount;
            }
            set
            {

                if (value != null)
                {
                    _amount = Math.Round((double)value, 3);
                }
                else
                {
                    _amount = null;
                }

            }
        }
    }
}
