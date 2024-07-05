using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    public class BuildupImage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonIgnore]
        public byte[] ImageData { get; set; }

    }
}
