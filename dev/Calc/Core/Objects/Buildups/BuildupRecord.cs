using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    /// <summary>
    /// record to serialize and save back to revit/rhino
    /// deserialize when got from a new selection
    /// and restore the buildup in current app
    /// </summary>
    public class BuildupRecord
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("buildup_group_id")]
        public int BuildupGroupId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("components")]
        public List<BuildupComponent> Components { get; set; }

        public object SerializeRecord()
        {
            return new
            {
                name = Name,
                buildup_group_id = BuildupGroupId,
                description = Description,
                components = Components.Select(c => c.SerializeRecord()).ToList()
            };
        }    
    }
}
