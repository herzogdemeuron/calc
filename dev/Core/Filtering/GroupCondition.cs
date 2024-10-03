using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Calc.Core.Objects.Elements;

namespace Calc.Core.Filtering
{
    public class GroupCondition
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

        public HashSet<string> GetAllParameters()
        {
            var parameters = new HashSet<string>();

            foreach (var conditionContainer in Conditions)
            {
                if (conditionContainer.Type.Equals("GroupCondition", StringComparison.OrdinalIgnoreCase))
                {
                    GroupCondition groupCondition = new GroupCondition
                    {
                        Operator = conditionContainer.Operator,
                        Conditions = conditionContainer.Conditions
                    };
                    parameters.UnionWith(groupCondition.GetAllParameters());
                }
                else if (conditionContainer.Type.Equals("SimpleCondition", StringComparison.OrdinalIgnoreCase))
                {
                    SimpleCondition simpleCondition = conditionContainer.Condition;
                    {
                        parameters.Add(simpleCondition.Parameter);
                    }
                }
            }
            return parameters;
        }
    }
}


