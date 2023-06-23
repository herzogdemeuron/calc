using Speckle.Newtonsoft.Json;
using Speckle.Newtonsoft.Json.Serialization;

namespace Calc.Core.DirectusAPI
{
    public class IdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
