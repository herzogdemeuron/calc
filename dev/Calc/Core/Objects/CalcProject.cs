using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Objects
{
    public class CalcProject : IShowName
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("lca_method")]
        public string LcaMethod { get; set; }
        [JsonProperty("area")]
        public decimal Area { get; set; }
        [JsonProperty("life_span")]
        public int? LifeSpan { get; set; }
        [JsonProperty("stages")]
        public List<string> Stages { get; set; }
        [JsonProperty("impact_categories")]
        public List<string> ImpactCategories { get; set; }
        [JsonIgnore]
        public string ShowName => $"{Number} - {Name}";
    }
}