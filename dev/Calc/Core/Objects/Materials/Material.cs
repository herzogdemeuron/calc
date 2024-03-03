using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class Material
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; } = "";

        [JsonProperty("material_category")]
        public string Category { get; set; }

        [JsonProperty("material_name")]
        public string Name { get; set; }

        [JsonProperty("source_db")]
        public string Source { get; set; }

        [JsonProperty("source_db_identifier")]
        public string SourceCode { get; set; }

        [JsonProperty("valid_until")]
        public int ValidUntil { get; set; }

        [JsonProperty("valid_from")]
        public int ValidFrom { get; set; }

        [JsonProperty("thickness")]
        public double Thickness { get; set; }

        [JsonProperty("gross_density")]
        public int GrossDensity { get; set; }

        [JsonProperty("bulk_density")]
        public int BulkDensity { get; set; }

        [JsonProperty("grammage")]
        public int Grammage { get; set; }

        [JsonProperty("linear_density")]
        public int LinearDensity { get; set; }

        [JsonProperty("piece_quantity")]
        public int PieceQuantity { get; set; } = 0;

        [JsonProperty("global_warming_potential_a1_a2_a3")]
        public double GWP { get; set; }

        [JsonProperty("grey_energy_a123")]
        public double GE { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, Category: {Category}";
        }
    }

}
