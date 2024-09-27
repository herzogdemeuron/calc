using Newtonsoft.Json;

namespace Calc.Core.Objects.Materials
{
    public class MaterialFunction
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("amortization")]
        public int Amortization { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is MaterialFunction mFunction)
            {
                return mFunction.Name == Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
