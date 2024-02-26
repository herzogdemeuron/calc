using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialGroup
    {
        [JsonProperty("group_name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Group Name: {Name}";
        }
    }
}
