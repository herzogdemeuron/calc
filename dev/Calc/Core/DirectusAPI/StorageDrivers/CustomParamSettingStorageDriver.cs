﻿using Calc.Core.Objects.Elements;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class CustomParamSettingStorageDriver : IDriverGetMany<CustomParamSetting>
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

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
