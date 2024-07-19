using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Objects.Buildups
{
    /// <summary>
    /// record to serialize and save back to revit/rhino
    /// deserialize when got from a new selection
    /// and restore the buildup in current app
    /// </summary>
    public class BuildupRecord
    {
        [JsonProperty("buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty("buildup_group")]
        public BuildupGroup BuildupGroup { get; set; }
        [JsonProperty("buildup_unit")]
        public Unit BuildupUnit { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("components")]
        public List<BuildupComponent> Components { get; set; }

        public object SerializeRecord()
        {
            return new
            {
                buildup_name = BuildupName,
                buildup_group = BuildupGroup,
                buildup_unit = BuildupUnit,
                description = Description,
                components = Components
                .Where(c => c.HasMaterial || c.IsNormalizer)
                .Select(c => c.SerializeRecord())
                .ToList()
            };
        }    
    }
}
