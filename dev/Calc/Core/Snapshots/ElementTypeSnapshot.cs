using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// one element type with each meta data and the material layers
    /// </summary>
    public class ElementTypeSnapshot
    {
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; } = new List<string>();
        [JsonProperty("element_amount")]
        public double ElementAmount { get; set; } // uses the assembly unit
        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; }
        [JsonIgnore]
        public double? TotalGwp => MaterialSnapshots.Sum(m => m.CalculatedGwp);
        [JsonIgnore]
        public double? TotalGe => MaterialSnapshots.Sum(m => m.CalculatedGe);

        public ElementTypeSnapshot Copy()
        {
            return new ElementTypeSnapshot
            {
                ElementTypeId = ElementTypeId,
                ElementIds = ElementIds,
                ElementAmount = ElementAmount,
                MaterialSnapshots = MaterialSnapshots.ConvertAll(m => m.Copy())
            };
            
        }
    }
}
