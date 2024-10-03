using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Filtering
{
    /// <summary>
    /// Contains either a GroupCondition or a SimpleCondition as type.
    /// </summary>
    public class ConditionContainer
    {
        [JsonProperty("type")]
        public string Type { get; set; } // either "group_condition" or "simple_condition"

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("conditions")]
        public List<ConditionContainer> Conditions { get; set; }

        [JsonProperty("condition")]
        public SimpleCondition Condition { get; set; }
    }
}
