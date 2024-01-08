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
        public string FormattedKgCO2eA123 { get => Math.Round(Amount * Material.KgCO2eA123, 2).ToString() + " Kg"; }
        [JsonIgnore]
        public string FormattedCost { get => Math.Round(Amount * Material.Cost, 2).ToString(); }
    }
}
