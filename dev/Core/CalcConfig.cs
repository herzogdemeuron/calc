using Newtonsoft.Json;

namespace Calc.Core
{
    public class CalcConfig
    {
        [JsonProperty("speckle_token")]
        public string SpeckleToken { get; set; }
        [JsonProperty("speckle_server_url")]
        public string SpeckleServerUrl { get; set; }
        [JsonProperty("speckle_builder_project_id")]
        public string SpeckleBuilderProjectId { get; set; }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SpeckleToken) &&
                       !string.IsNullOrWhiteSpace(SpeckleServerUrl) &&
                       !string.IsNullOrWhiteSpace(SpeckleBuilderProjectId);
            }
        }
    }
}
