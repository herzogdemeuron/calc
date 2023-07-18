using System.Collections.Generic;
using System;
using Calc.Core.Objects;
using Speckle.Newtonsoft.Json;


namespace Calc.Core.Filtering;

public class ElementFilter
{
    public List<CalcElement> FilterElements(List<CalcElement> elements, GroupCondition filter)
    {
        List<CalcElement> filteredElements = new List<CalcElement>();

        foreach (CalcElement element in elements)
        {
            if (filter.Evaluate(element))
            {
                filteredElements.Add(element);
            }
        }

        return filteredElements;
    }
}


public interface ICondition
{
    public bool Evaluate(CalcElement element);
}


public class GroupCondition : ICondition
{
    [JsonProperty("operator")]
    public string Operator { get; set; }

    [JsonProperty("conditions")]
    public List<ConditionContainer> Conditions { get; set; }

    public bool Evaluate(CalcElement element)
    {
        if (Operator.Equals("and", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var conditionContainer in Conditions)
            {
                if (conditionContainer.Type.Equals("GroupCondition", StringComparison.OrdinalIgnoreCase))
                {
                    GroupCondition groupCondition = new GroupCondition
                    {
                        Operator = conditionContainer.Operator,
                        Conditions = conditionContainer.Conditions
                    };
                    if (!groupCondition.Evaluate(element))
                    {
                        return false;
                    }
                }
                else if (conditionContainer.Type.Equals("SimpleCondition", StringComparison.OrdinalIgnoreCase))
                {
                    SimpleCondition simpleCondition = conditionContainer.Condition;
                    if (!simpleCondition.Evaluate(element))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else if (Operator.Equals("or", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var conditionContainer in Conditions)
            {
                if (conditionContainer.Type.Equals("GroupCondition", StringComparison.OrdinalIgnoreCase))
                {
                    GroupCondition groupCondition = new GroupCondition
                    {
                        Operator = conditionContainer.Operator,
                        Conditions = conditionContainer.Conditions
                    };
                    if (groupCondition.Evaluate(element))
                    {
                        return true;
                    }
                }
                else if (conditionContainer.Type.Equals("SimpleCondition", StringComparison.OrdinalIgnoreCase))
                {
                    SimpleCondition simpleCondition = conditionContainer.Condition;
                    if (simpleCondition.Evaluate(element))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        return false;
    }
}


public class SimpleCondition : ICondition
{
    [JsonProperty("parameter")]
    public string Parameter { get; set; }

    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("value")]
    public object Value { get; set; }

    public bool Evaluate(CalcElement element)
    {
        if (!element.Fields.TryGetValue(Parameter, out object fieldValue))
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


public class ConditionContainer
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("operator")]
    public string Operator { get; set; }

    [JsonProperty("conditions")]
    public List<ConditionContainer> Conditions { get; set; }

    [JsonProperty("condition")]
    public SimpleCondition Condition { get; set; }
}

