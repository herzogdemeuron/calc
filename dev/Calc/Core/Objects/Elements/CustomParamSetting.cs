using Newtonsoft.Json;

namespace Calc.Core.Objects.Elements
{
    /// <summary>
    /// This is the parameter setting used for calc element creation
    /// defines which custom parameter names are used from the native elements
    /// </summary>
    public class CustomParamSetting
    {
        [JsonProperty("connector")]
        public string Connector { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("length_custom_param_name")]
        public string LengthCustomParamName { get; set; }
        [JsonProperty("area_custom_param_name")]
        public string AreaCustomParamName { get; set; }
        [JsonProperty("volume_custom_param_name")]
        public string VolumeCustomParamName { get; set; }
    }



}
