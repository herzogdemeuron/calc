using Calc.Core.Objects.Materials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Assemblies
{
    public class AssemblyGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is AssemblyGroup)
            {
                return (obj as AssemblyGroup).Name == Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
