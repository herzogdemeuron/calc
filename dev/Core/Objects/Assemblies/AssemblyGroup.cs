using Newtonsoft.Json;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// Assembly group, collection defined in directus.
    /// </summary>
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
