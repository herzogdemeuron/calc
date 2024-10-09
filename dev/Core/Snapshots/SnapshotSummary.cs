using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// A summary of the query snapshots and the total GWP and GE.
    /// </summary>
    public class SnapshotSummary
    {
        [JsonProperty("total_gwp")]
        public double TotalGwp { get; set; }
        [JsonProperty("total_ge")]
        public double TotalGe { get; set; }
        [JsonProperty("queries")]
        public List<QuerySnapshot> QuerySnapshots { get; set; }
    }
}
