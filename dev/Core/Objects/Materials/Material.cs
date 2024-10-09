using Calc.Core.Objects.Standards;
using Newtonsoft.Json;

namespace Calc.Core.Objects.Materials
{
    public class Material : ISearchable
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("standard")]
        public LcaStandard Standard { get; set; }
        [JsonProperty("material_type_family")]
        public string MaterialTypeFamily { get; set; }
        [JsonProperty("material_type")]
        public string MaterialType { get; set; }
        [JsonProperty("product_type_family")]
        public string ProductTypeFamily { get; set; }
        [JsonProperty("product_type")]
        public string ProductType { get; set; }
        [JsonProperty("data_source")]
        public string DataSource { get; set; }
        [JsonProperty("source_uuid")]
        public string SourceUuid { get; set; }
        [JsonProperty("thickness")]
        public double? Thickness { get; set; }
        [JsonProperty("material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty("updated")]
        public string Updated { get; set; }
        [JsonProperty("carbon_a1a3")]
        public double? Gwp { get; set; }
        [JsonProperty("grey_energy_fabrication_total")]
        public double? Ge { get; set; }
        public string GroupName => MaterialType; // for searching
    }

}
