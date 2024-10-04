using Calc.Core.Objects.Standards;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get standards from directus.
    /// </summary>
    public class StandardDriver : IDriverGetMany<LcaStandard>
    {
        public string QueryGetMany { get; } = @"
            query GetStandards {
                calc_standards {
                    id
                    name
                }
            }";

        [JsonProperty("calc_standards")]
        public List<LcaStandard> GotManyItems { get; set; }
    }
}
