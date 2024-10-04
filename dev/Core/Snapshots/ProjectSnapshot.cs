using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// Should be serialized and saved to directus as json file.
    /// </summary>
    public class ProjectSnapshot
    {
        [JsonProperty("project_number")]
        public string ProjectNumber { get; set; }
        [JsonProperty("project_name")]
        public string ProjectName { get; set; }
        [JsonProperty("query_template")]
        public string QueryTemplate { get; set; }
        [JsonProperty("location")]
        public string? Location { get; set; }
        [JsonProperty("lca_method")]
        public string? LcaMethod { get; set; }
        [JsonProperty("life_span")]
        public int? LifeSpan { get; set; }
        [JsonProperty("stages")]
        public List<string> Stages { get; set; }
        [JsonProperty("impact_categories")]
        public List<string> ImpactCategories { get; set; }
        [JsonProperty("assemblies")]
        public List<AssemblySnapshot> AssemblySnapshots { get; set; }
    }
}
