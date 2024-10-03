using Calc.Core.Objects.GraphNodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get forests from directus.
    /// </summary>
    public class ForestStorageDriver : IDriverGetMany<Forest>
    {
        public Forest SendItem { get; set; }

        public string QueryGetMany { get; } = @"
                    query GetAllForests {
                        calc_forests {
                        id
                        forest_name
                        trees
                        project_id {
                            id
                            name
                            number
                        }
                        }
                    }";
        [JsonProperty("calc_forests")]
        public List<Forest> GotManyItems { get; set; }
    }
}
