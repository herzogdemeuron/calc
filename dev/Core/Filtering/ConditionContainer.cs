using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Filtering
{
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
}
