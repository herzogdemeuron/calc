using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialReference
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
