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
        public double MaterialAmount { get; set; }
        [JsonProperty("material_carbon_a1a3")]
        public double? MaterialGwp { get; set; } // the unit gwp of the material
        [JsonProperty("material_grey_energy_fabrication_total")]
        public double? MaterialGe { get; set; } // the unit ge of the material, for validation
        [JsonProperty("calculated_carbon_a1a3")]
        public double? CalculatedGwp { get; set; }
        [JsonProperty("calculated_grey_energy_fabrication_total")]
        public double? CalculatedGe { get; set; }

    
        /// <summary>
        /// when claiming the element, the element amount ratio means
        /// how many times buildup unit does this element have,
        /// apply the ratio to the snapshot, calculated gwp and ge are multiplied
        /// </summary>
        public void ApplyAmountRatio(double amountRatio)
        {
            MaterialAmount = MaterialAmount * amountRatio;
            if (MaterialGwp.HasValue) CalculatedGwp = MaterialGwp * MaterialAmount; // this should equal CalculatedGwp * amountRatio
            if (MaterialGe.HasValue) CalculatedGe = MaterialGe * MaterialAmount;
        }

        public MaterialSnapshot Copy()
            {
            return new MaterialSnapshot
            {
                MaterialFunction = MaterialFunction,
                MaterialSourceUuid = MaterialSourceUuid,
                MaterialSource = MaterialSource,
                MaterialName = MaterialName,
                MaterialUnit = MaterialUnit,
                MaterialAmount = MaterialAmount,
                MaterialGwp = MaterialGwp,
                MaterialGe = MaterialGe,
                CalculatedGwp = CalculatedGwp,
                CalculatedGe = CalculatedGe
            };
        }
    }
}
