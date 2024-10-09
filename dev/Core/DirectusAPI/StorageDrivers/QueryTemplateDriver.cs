using Calc.Core.Objects.GraphNodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get query templates from directus.
    /// </summary>
    public class QueryTemplateDriver : IDriverGetMany<QueryTemplate>
    {
        public QueryTemplate SendItem { get; set; }

        public string QueryGetMany { get; } = @"
                    query GetAllQueryTemplates {
                        calc_query_templates {
                        id
                        name
                        queries
                        project_id {
                            id
                            name
                            number
                        }
                        }
                    }";
        [JsonProperty("calc_query_templates")]
        public List<QueryTemplate> GotManyItems { get; set; }
    }
}
