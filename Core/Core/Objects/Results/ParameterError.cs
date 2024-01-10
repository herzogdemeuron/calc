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



    }
}
