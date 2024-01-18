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
        public decimal ElementQuantity { get; set; }

        // building infos
        [JsonProperty(PropertyName = "buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty(PropertyName = "buildup_group")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "buildup_unit")]
        public Unit BuildupUnit { get; set; }

        // material infos
        [JsonProperty(PropertyName = "material_name")]
        public string MaterialName { get; set; }
        [JsonProperty(PropertyName = "material_category")]
        public string MaterialCategory { get; set; }
        [JsonProperty(PropertyName = "material_source")]
        public string MaterialSource { get; set; }
        [JsonProperty(PropertyName = "material_source_code")]
        public string MaterialSourceCode { get; set; }
        [JsonProperty(PropertyName = "material_gwp")]
        public decimal MaterialGwp { get; set; }
        [JsonProperty(PropertyName = "material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty(PropertyName = "material_amount")]
        public decimal MaterialAmount { get; set; }

        [JsonProperty(PropertyName = "calculated_gwp")]
        public decimal Gwp { get; set; }
        [JsonProperty(PropertyName = "calculated_ge")]
        public decimal Ge { get; set; }
        [JsonProperty(PropertyName = "calculated_cost")]
        public decimal Cost { get; set; }

        // others
        [JsonProperty(PropertyName = "color")]
        public HslColor Color { get; set; }
    }
}
