using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// record to serialize and save back to revit/rhino
    /// deserialize when got from a new selection
    /// and restore the assembly in current app
    /// </summary>
    public class AssemblyRecord
    {
        [JsonProperty("assembly_name")]
        public string BuildupName { get; set; }
        [JsonProperty("assembly_group")]
        public AssemblyGroup BuildupGroup { get; set; }
        [JsonProperty("assembly_unit")]
        public Unit BuildupUnit { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("components")]
        public List<AssemblyComponent> Components { get; set; }

        public object SerializeRecord()
        {
            return new
            {
                assembly_name = BuildupName,
                assembly_group = BuildupGroup,
                assembly_unit = BuildupUnit,
                description = Description,
                components = Components.Select(c => c.SerializeRecord()).ToList()
            };
        }    
    }
}
