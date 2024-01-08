using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class Material
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; } = -1;
        [JsonProperty("material_name")]
        public string Name { get; set; }
        [JsonProperty("global_warming_potential_a1_a2_a3")]
        public decimal KgCO2eA123 { get; set; }
        [JsonProperty("cost")]
        public decimal Cost { get; set; }
        [JsonProperty("unit")]
        public Unit Unit { get; set; }
        [JsonProperty("material_category")]
        public string Category { get; set; }
        [JsonProperty("source_db")]
        public string Source { get; set; }
        [JsonProperty("source_db_identifier")]
        public string SourceCode { get; set; }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, KgCO2eA123: {KgCO2eA123}, Category: {Category}";
        }
    }

}
