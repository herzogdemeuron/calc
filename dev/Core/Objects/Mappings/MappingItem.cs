using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Mappings
{

    /// <summary>
    /// Stands for a set of assemblies that are assigned to a branch via a path
    /// </summary>
    public class MappingItem
    {
        [JsonProperty("assembly_ids")]
        public List<int> AssemblyIds { get; set; }
        [JsonProperty("mapping_path")]
        public List<MappingPath> Path { get; set; }
        [JsonProperty("query_name")]
        public string QueryName { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
