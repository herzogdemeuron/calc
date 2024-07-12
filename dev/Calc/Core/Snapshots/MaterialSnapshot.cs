using Calc.Core.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// material snapshots should be grouped and merged by material function, source uuid and the source
    /// </summary>
    public class MaterialSnapshot
    {
        [JsonProperty("material_function")]
        public string MaterialFunction { get; set; }
        [JsonProperty("material_source_uuid")]
        public string MaterialSourceUuid { get; set; }
        [JsonProperty("material_source")]
        public string MaterialSource { get; set; }
        [JsonProperty("material_name")]
        public string MaterialName { get; set; }
        [JsonProperty("material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty("material_amount")]
        public double MaterialAmount { get; set; } // the total amount of the whole buildup amount
        [JsonProperty("material_carbon_a1a3")]
        public double? MaterialGwp { get; set; } // the unit gwp of the material
        [JsonProperty("material_grey_energy_fabrication_total")]
        public double? MaterialGe { get; set; } // the unit ge of the material
        [JsonProperty("calculated_carbon_a1a3")]
        public double? Gwp { get; set; }
        [JsonProperty("calculated_grey_energy_fabrication_total")]
        public double? Ge { get; set; }

    
        public void ApplyRatio(double ratio)
        {
            MaterialAmount = MaterialAmount * ratio;
            if (MaterialGwp.HasValue) Gwp = MaterialGwp * MaterialAmount;
            if (MaterialGe.HasValue) Ge = MaterialGe * MaterialAmount;
        }
    }
}
