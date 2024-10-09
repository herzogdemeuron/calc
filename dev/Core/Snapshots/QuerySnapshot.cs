using Newtonsoft.Json;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// The calculation snapshot of a query.
    /// </summary>
    public class QuerySnapshot
    {
        [JsonProperty("query")]
        public string QueryName { get; set; }
        [JsonProperty("gwp")]
        public double Gwp { get; set; }
        [JsonProperty("ge")]
        public double Ge { get; set; }
    }
}
