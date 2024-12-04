using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// Record of an assembly, to serialize and save back to revit/rhino,
    /// could be deserialized when read from a new selection (revit group),
    /// and restore the assembly in current app.
    /// </summary>
    public class AssemblyRecord
    {
        [JsonProperty("assembly_group")]
        public AssemblyGroup AssemblyGroup { get; set; }
        [JsonProperty("assembly_unit")]
        public Unit AssemblyUnit { get; set; }
        [JsonProperty("components")]
        public List<AssemblyComponent> Components { get; set; }

        /// <summary>
        /// Serializes the current assembly record, as well all assembly components.
        /// </summary>
        public object SerializeRecord()
        {
            return new
            {
                assembly_group = AssemblyGroup,
                assembly_unit = AssemblyUnit,
                components = Components.Select(c => c.SerializeRecord()).ToList()
            };
        }    
    }
}
