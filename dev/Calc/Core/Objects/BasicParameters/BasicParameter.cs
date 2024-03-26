using Calc.Core.Objects.Results;
using Speckle.Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
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

        public bool HasError => ErrorType != null;
        public bool CanOperate => !HasError || ErrorType == ParameterErrorType.ZeroValue; // zero value allows operations

        public bool HasValue => Amount != null && Amount >= 0;

        public BasicParameter PerformOperation(Operation operation, BasicParameter otherParam)
        {
            var resultParam = new BasicParameter()
            {
                Name = Name,
                Unit = Unit
            };

            if(!CanOperate && otherParam.CanOperate)
            {
                resultParam.ErrorType = ErrorType;
                return resultParam;
            }

            if (CanOperate && !otherParam.CanOperate)
            {
                resultParam.ErrorType = otherParam.ErrorType;
                return resultParam;
            }

            if (!CanOperate && !otherParam.CanOperate)
            {
                resultParam.ErrorType = ErrorType == otherParam.ErrorType? ErrorType : ParameterErrorType.CalculationError;
                return resultParam;
            }

            if (!HasValue || !otherParam.HasValue)
            {
                resultParam.ErrorType = ParameterErrorType.CalculationError;
                return resultParam;
            }

            double amount1 = (double)Amount;
            double amount2 = (double)otherParam.Amount;

            var resultAmount = Operate(amount1, operation, amount2);
            if (resultAmount == null)
            {
                resultParam.ErrorType = ParameterErrorType.CalculationError;
            }
            else
            {
                resultParam.Amount = resultAmount;
            }

            if (resultParam.Amount == 0)
            {
                resultParam.ErrorType = ParameterErrorType.ZeroValue;
            }

            return resultParam;
        }

        public BasicParameter PerformOperation(Operation operation, double value)
        {
            var resultParam = new BasicParameter()
            {
                Name = Name,
                Unit = Unit
            };

            if (HasError)
            {
                resultParam.ErrorType = ErrorType;
                return resultParam;
            }

            if(!HasValue)
            {
                resultParam.ErrorType = ParameterErrorType.CalculationError;
                return resultParam;
            }

            double amount1 = (double)Amount;
            var resultAmount = Operate(amount1, operation, value);

            if(resultAmount == null)
            {
                resultParam.ErrorType = ParameterErrorType.CalculationError;
            }
            else
            {
                resultParam.Amount = resultAmount;
            }

            return resultParam;
        }

        private double? Operate(double amount1, Operation operation, double amount2)
        {
            switch (operation)
            {
                case Operation.Add:
                    return amount1 + amount2;
                case Operation.Subtract:
                    return amount1 - amount2;
                case Operation.Multiply:
                    return amount1 * amount2;
                case Operation.Divide:
                    if (amount2 == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return amount1 / amount2;
                    }
            }
            return null;
        }
    }
}
