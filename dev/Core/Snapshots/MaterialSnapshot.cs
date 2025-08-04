using Calc.Core.Objects;
using Newtonsoft.Json;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// Snapshot for a material layer, which material properties and the calculated results.
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
        /// When claiming the element, the element amount ratio means
        /// how many times assembly unit does this element have,
        /// apply the ratio to the snapshot, calculated gwp and ge are multiplied.
        /// </summary>
        internal void ApplyAmountRatio(double amountRatio)
        {
            MaterialAmount = MaterialAmount * amountRatio;
            // use calculated snapshot instead of material kpi
            if (MaterialGwp.HasValue) CalculatedGwp = CalculatedGwp * amountRatio;
            if (MaterialGe.HasValue) CalculatedGe = CalculatedGe * amountRatio;
        }

        /// <summary>
        /// Merges a material snapshot to this, adding up the amount and kpis.
        /// The equality should be already checked.
        /// </summary>
        internal void Merge(MaterialSnapshot mSnapshot)
        {
            MaterialAmount += mSnapshot.MaterialAmount;
            CalculatedGwp += mSnapshot.CalculatedGwp;
            CalculatedGe += mSnapshot.CalculatedGe;
        }

        /// <summary>
        /// Compares if the two material snapshots should be equally categorized
        /// with material function, source uuid and source.
        /// </summary>
        internal bool Equals(MaterialSnapshot mSnapshot)
        {
            return MaterialSourceUuid == mSnapshot.MaterialSourceUuid &&
                   MaterialSource == mSnapshot.MaterialSource &&
                   MaterialFunction == mSnapshot.MaterialFunction;
        }

        internal MaterialSnapshot Copy()
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
