using Newtonsoft.Json;

namespace Calc.MVVM.Models
{
    public class CategorizedResultModel // deprecated
    {
        [JsonProperty("query")]
        public string Group { get; set; }
        [JsonProperty("gwp")]
        public double Gwp { get; set; }
        [JsonProperty("ge")]
        public double Ge { get; set; }
    }
}
