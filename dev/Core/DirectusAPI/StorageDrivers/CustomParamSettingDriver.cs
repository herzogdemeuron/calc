using Calc.Core.Objects.Elements;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get param settings from directus.
    /// </summary>
    public class CustomParamSettingDriver : IDriverGetMany<CustomParamSetting>
    {
        public string QueryGetMany { get; } = @"
            query GetParamSettings {
                calc_param_settings(limit: 200) {
                    connector
                    category
                    length_custom_param_name
                    area_custom_param_name
                    volume_custom_param_name
                }
            }";

        [JsonProperty("calc_param_settings")]
        public List<CustomParamSetting> GotManyItems { get; set; }
    }
}
