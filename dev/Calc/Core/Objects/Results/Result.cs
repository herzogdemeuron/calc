using Calc.Core.Color;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Results
{

    public class Result
    {
        // parent infos
        [JsonProperty(PropertyName = "forest")]
        public string Forest { get; set; }
        [JsonProperty(PropertyName = "tree")]
        public string Tree { get; set; }

        // element infos
        [JsonProperty(PropertyName = "element_id")]
        public string ElementId { get; set; }
        [JsonProperty(PropertyName = "element_type")]
        public string ElementType { get; set; }
        [JsonProperty(PropertyName = "element_unit")]
        public Unit ElementUnit { get; set; }
        [JsonProperty(PropertyName = "element_quantity")]
        public double ElementQuantity { get; set; }

        // buildup infos
        [JsonProperty(PropertyName = "buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty(PropertyName = "buildup_group")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "buildup_unit")]
        public Unit BuildupUnit { get; set; }

        // material key data
        [JsonProperty(PropertyName = "material_name")]
        public string MaterialName { get; set; }

        [JsonProperty(PropertyName = "material_unit")]
        public Unit MaterialUnit { get; set; }

        [JsonProperty(PropertyName = "material_amount")]
        public double MaterialAmount { get; set; }

        [JsonProperty(PropertyName = "material_standard")]
        public string MaterialStandard { get; set; }

        [JsonProperty(PropertyName = "material_source")]
        public string MaterialSource { get; set; }

        [JsonProperty(PropertyName = "material_source_uuid")]
        public string MaterialSourceUuid { get; set; }

        [JsonProperty(PropertyName = "material_function")]
        public string MaterialFunction { get; set; }

        // material values
        [JsonProperty(PropertyName = "material_carbon_a1a3")]
        public double MaterialGwp { get; set; }
        [JsonProperty(PropertyName = "material_grey_energy_fabrication_total")]
        public double MaterialGe { get; set; }

        // material layer calculation
        [JsonProperty(PropertyName = "calculated_carbon_a1a3")]
        public double Gwp { get; set; }
        [JsonProperty(PropertyName = "calculated_grey_energy_fabrication_total")]
        public double Ge { get; set; }
        [JsonProperty(PropertyName = "calculated_cost")]
        public double Cost { get; set; }

        // others
        [JsonProperty(PropertyName = "color")]
        public HslColor Color { get; set; }
    }
}
