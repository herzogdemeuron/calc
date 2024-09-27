﻿using Newtonsoft.Json;
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
        public string AssemblyName { get; set; }
        [JsonProperty("assembly_group")]
        public AssemblyGroup AssemblyGroup { get; set; }
        [JsonProperty("assembly_unit")]
        public Unit AssemblyUnit { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("components")]
        public List<AssemblyComponent> Components { get; set; }

        public object SerializeRecord()
        {
            return new
            {
                assembly_name = AssemblyName,
                assembly_group = AssemblyGroup,
                assembly_unit = AssemblyUnit,
                description = Description,
                components = Components.Select(c => c.SerializeRecord()).ToList()
            };
        }    
    }
}
