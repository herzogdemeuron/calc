using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Results
{

    public class Snapshot : IHasProject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("project")]
        public Project Project { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "result_file")]
        public string JsonUuid { get; set; }
    }

}
