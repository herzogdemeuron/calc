using Newtonsoft.Json;

namespace Calc.Core.Objects.Standards
{
    public class LcaStandard : IShowName
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonIgnore]
        public string ShowName { get => Name; }

        public override bool Equals(object obj)
        {
            if (obj is LcaStandard standard)
            {
                return standard.Name == Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}
