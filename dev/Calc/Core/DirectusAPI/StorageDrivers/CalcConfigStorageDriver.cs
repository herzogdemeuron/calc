using Calc.Core.DirectusAPI.Drivers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core
{
    public class CalcConfigStorageDriver : IDriverGetSingle<CalcConfig>
    {
        public string QueryGetSingle { get; } = @"
            query GetCalcConfig {
                calc_config {
                    speckle_server_url
                    speckle_token
                    speckle_builder_project_id
                }
            }";

        [JsonProperty("calc_config")]
        public CalcConfig GotItem { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
