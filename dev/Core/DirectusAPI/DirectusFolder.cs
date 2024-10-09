using Newtonsoft.Json;

namespace Calc.Core.DirectusAPI
{
    internal class DirectusFolder
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("name")]
        internal string Name { get; set; }
    }
}
