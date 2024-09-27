using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Mappings
{

    /// <summary>
    /// stands for a set of assemblies that are assigned to a branch via a path
    /// </summary>
    public class MappingItem
    {
        [JsonProperty("assembly_ids")]
        public List<int> AssemblyIds { get; set; }
        [JsonProperty("mapping_path")]
        public List<MappingPath> Path { get; set; }

        [JsonProperty("tree_name")]
        public string TreeName { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
