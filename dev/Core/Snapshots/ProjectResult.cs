using Calc.Core.Objects;
using Newtonsoft.Json;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// Includes the project snapshot (saved to directus as json file) and the meta data.
    /// </summary>
    public class ProjectResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("project")]
        public CalcProject Project { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("snapshot_file")]
        public string JsonUuid { get; set; }
    }

}
