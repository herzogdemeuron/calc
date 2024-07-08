using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects;
using Calc.Core.Objects.Elements;

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
