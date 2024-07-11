using Calc.Core.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    public class BuildupSnapshot
    {
        [JsonProperty("element_group")]
        public string ElementGroup { get; set; } // tree
        [JsonProperty("buidup_name")]
        public string BuildupName { get; set; }
        [JsonProperty("buildup_code")]
        public string BuildupCode { get; set; }
        [JsonProperty("buildup_group")]
        public string BuildupGroup { get; set; }
        [JsonProperty("buildup_unit")]
        public string BuildupUnit { get; set; }
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; }
        [JsonProperty("element_unit")]
        public string ElementUnit { get; set; }
        [JsonProperty("element_amount")]
        public double ElementAmount { get; set; }
        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; }

    }
}
