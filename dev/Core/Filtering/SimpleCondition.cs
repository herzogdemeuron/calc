using Calc.Core.Objects.Elements;
using Newtonsoft.Json;

namespace Calc.Core.Filtering
{
    /// <summary>
    /// Represents a simple condition that can be evaluated against a CalcElement.
    /// </summary>
    public class SimpleCondition
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }

        public bool Evaluate(CalcElement element)
        {
            if (!element.Fields.TryGetValue(Parameter, out object fieldValue) || fieldValue == null)
            {
                return false;
            }
            switch (Method.ToLowerInvariant())
            {
                case "equals":
                    return fieldValue.Equals(Value);
                case "notequals":
                    return !fieldValue.Equals(Value);
                case "contains":
                    return fieldValue.ToString().Contains(Value.ToString());
                case "notcontains":
                    return !fieldValue.ToString().Contains(Value.ToString());
                case "startswith":
                    return fieldValue.ToString().StartsWith(Value.ToString());
                case "notstartswith":
                    return !fieldValue.ToString().StartsWith(Value.ToString());
                case "endswith":
                    return fieldValue.ToString().EndsWith(Value.ToString());
                case "notendswith":
                    return !fieldValue.ToString().EndsWith(Value.ToString());
                case "greaterthan":
                    return (double)fieldValue > (double)Value;
                case "graterthanorequalto":
                    return (double)fieldValue >= (double)Value;
                case "lessthan":
                    return (double)fieldValue < (double)Value;
                case "lessthanorequalto":
                    return (double)fieldValue <= (double)Value;
                // Add more cases for other supported methods if needed

                default:
                    return false;
            }
        }
    }
}
