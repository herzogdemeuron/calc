using Calc.Core.Objects;
using Newtonsoft.Json;

namespace Calc.Core.Snapshots
{

    public class ProjectResult : IHasProject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("project")]
        public CalcProject Project { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "result_file")]
        public string JsonUuid { get; set; }
    }

}
