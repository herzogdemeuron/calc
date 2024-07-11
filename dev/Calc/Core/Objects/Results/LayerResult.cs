using Newtonsoft.Json;

namespace Calc.Core.Objects.Results
{

    public class LayerResult
    {
        // parent infos
        [JsonProperty("forest")]
        public string Forest { get; set; }
        [JsonProperty("tree")]
        public string Tree { get; set; }

        // element infos
        [JsonProperty("element_id")]
        public string ElementId { get; set; }
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_type")]
        public string ElementType { get; set; }
        [JsonProperty("element_unit")]
        public Unit ElementUnit { get; set; }
        [JsonProperty("element_amount")]
        public double ElementAmount { get; set; }

        // buildup infos
        [JsonProperty("buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty("buildup_code")]
        public string BuildupCode { get; set; }
        [JsonProperty("buildup_group")]
        public string GroupName { get; set; }
        [JsonProperty("buildup_unit")]
        public Unit BuildupUnit { get; set; }

        // material key data
        [JsonProperty("material_name")]
        public string MaterialName { get; set; }

        [JsonProperty("material_unit")]
        public Unit MaterialUnit { get; set; }

        [JsonProperty("material_amount")]
        public double MaterialAmount { get; set; }

        [JsonProperty("material_standard")]
        public string MaterialStandard { get; set; }

        [JsonProperty("material_source")]
        public string MaterialSource { get; set; }

        [JsonProperty("material_source_uuid")]
        public string MaterialSourceUuid { get; set; }

        [JsonProperty("material_function")]
        public string MaterialFunction { get; set; }

        // material values
        [JsonProperty("material_carbon_a1a3")]
        public double MaterialGwp { get; set; }
        [JsonProperty("material_grey_energy_fabrication_total")]
        public double MaterialGe { get; set; }

        // material layer calculation
        [JsonProperty("calculated_carbon_a1a3")]
        public double Gwp { get; set; }
        [JsonProperty("calculated_grey_energy_fabrication_total")]
        public double Ge { get; set; }

    }
}
