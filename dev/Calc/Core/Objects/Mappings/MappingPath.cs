using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Mappings
{
    public class MappingPath
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (MappingPath)obj;
            return Parameter == other.Parameter && Value == other.Value;
        }
        public override int GetHashCode()
        {
            return Parameter.GetHashCode() ^ Value.GetHashCode();
        }

    }
}
