using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Results
{

    public class Snapshot : IHasProject
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; } = -1;
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "project")]
        public Project Project { get; set; }
        [JsonProperty(PropertyName = "results")]
        public List<Result> Results { get; set; }
    }

}
