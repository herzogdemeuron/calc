using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get calc config from directus.
    /// </summary>
    internal class CalcConfigStorageDriver : IDriverGetSingle<CalcConfig>
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
    }
}
