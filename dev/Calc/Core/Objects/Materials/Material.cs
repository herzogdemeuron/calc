using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class Material : ISearchable
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("standard")]
        public LcaStandard Standard { get; set; }
        [JsonProperty("material_type")]
        public string MaterialType { get; set; }
        [JsonProperty("product_type")]
        public string ProductType { get; set; }
        [JsonProperty("thickness")]
        public double? Thickness { get; set; }

        // deprecated
/*        [JsonProperty("density")]
        public double? Density { get; set; }*/

        [JsonProperty("material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty("updated")]
        public string? Updated { get; set; }
        [JsonProperty("carbon_a1a3")]
        public double? Gwp { get; set; }

        [JsonProperty("grey_energy_fabrication_total")]
        public double? Ge { get; set; }

        [JsonProperty("cost")]
        public double? Cost { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public string GroupName => MaterialType;
        //public string SourceCode { get; set; }

        public void LinkStandard(List<LcaStandard> standards)
        {
            if (Standard != null)
            {
                Standard = standards.Find(s => s.Id == Standard.Id);
            }
        }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, Material Type: {MaterialType}";
        }
    }

}
