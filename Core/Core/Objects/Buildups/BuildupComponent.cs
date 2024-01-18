using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    public class BuildupComponent
    {
        [JsonProperty("calc_materials_id")]
        public Material Material { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonIgnore]
        public string FormattedAmount { get => Math.Round(Amount, 2).ToString() + " " + Material.Unit; }
        [JsonIgnore]
        public string FormattedGWP { get => Math.Round(Amount * Material.GWP, 2).ToString(); }
        [JsonIgnore]
        public string FormattedGE { get => Math.Round(Amount * Material.GE, 2).ToString(); }
        [JsonIgnore]
        public string FormattedCost { get => Math.Round(Amount * Material.Cost, 2).ToString(); }
    }
}
