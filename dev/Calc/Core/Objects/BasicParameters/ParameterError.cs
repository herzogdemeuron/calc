using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.BasicParameters
{
    public enum ParameterErrorType
    {
        Redundant,
        Missing,
        ZeroValue,
        CalculationError
    }
    public class ParameterError
    {
        public string ParameterName { get; set; }
        public Unit Unit { get; set; }
        public ParameterErrorType? ErrorType { get; set; }
        public List<string> ElementIds { get; set; }
        public int Count
        {
            get => ElementIds.Count;
        }

        public string ElementIdsString
        {
            get => string.Join(", ", ElementIds);
        }

        public string ErrorMessage
        {
            get
            {
                string unit = Unit switch
                {
                    Unit.m => "length",
                    Unit.m2 => "area",
                    Unit.m3 => "volume",
                    _ => "unknown",
                };

                string issue = ErrorType switch
                {
                    ParameterErrorType.Redundant => "is redundant",
                    ParameterErrorType.Missing => "is missing",
                    ParameterErrorType.ZeroValue => "has zero value",
                    _ => "has unknown issue",
                };

                return $"The {unit} parameter '{ParameterName}' {issue} in {Count} elements.";
            }
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        public ParameterError(ParameterError other)
        {
            ParameterName = other.ParameterName;
            Unit = other.Unit;
            ErrorType = other.ErrorType;
            ElementIds = new List<string>(other.ElementIds);
        }

        public ParameterError()
        {
            ElementIds = new List<string>();
        }
    }
}
