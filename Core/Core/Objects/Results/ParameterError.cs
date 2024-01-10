using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Results
{
    public enum ParameterErrorType
    {
        Redundant,
        Missing,
        ZeroValue
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
                string unit;
                switch (Unit)
                {
                    case Unit.m:
                        unit = "length";
                        break;
                    case Unit.m2:
                        unit = "area";
                        break;
                    case Unit.m3:
                        unit = "volume";
                        break;
                    default:
                        unit = "unknown";
                        break;
                }

                string issue;
                switch (ErrorType)
                {
                    case ParameterErrorType.Redundant:
                        issue = "is redundant";
                        break;
                    case ParameterErrorType.Missing:
                        issue = "is missing";
                        break;
                    case ParameterErrorType.ZeroValue:
                        issue = "has zero value";
                        break;
                    default:
                        issue = "has unknown issue";
                        break;
                }
                return $"The {unit} parameter '{ParameterName}' {issue} in {Count} elements.";
            }
}
    }
}
