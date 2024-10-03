using System;
using System.Collections.Generic;
using Calc.Core.Objects.Mappings;
using Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get / create mappings from directus.
    /// </summary>
    public class MappingStorageDriver : IDriverCreateSingle<Mapping>, IDriverGetMany<Mapping>, IDriverUpdateSingle<Mapping>
    {
        public Mapping SendItem { get; set; }
        public string QueryGetMany { get; } = @"
                    query GetAllMappings {
                        calc_mappings {
                            id
                            name
                            mappings
                            updated
                            project {
                                id
                                name
                                number
                            }
                        }
                    }";

        public string QueryCreateSingle { get; } = @"
            mutation CreateMapping($input: create_calc_mappings_input!) {
              create_calc_mappings_item(data: $input) {
                id
              }
            }";

        public string QueryUpdateSingle { get; } = @"
            mutation UpdateMapping($id: ID!, $input: update_calc_mappings_input!) {
              update_calc_mappings_item(id: $id, data: $input) {
                id
              }
            }";


        [JsonProperty("calc_mappings")]
        public List<Mapping> GotManyItems { get; set; }
        [JsonProperty("create_calc_mappings_item")]
        public Mapping CreatedItem { get; set; }
        [JsonProperty("update_calc_mappings_item")]
        public Mapping UpdatedItem { get; set; }

        /// <summary>
        /// Provides creation variables.
        /// </summary>
        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }

            var input = new
            {
                name = SendItem.Name,
                mappings = this.SendItem.SerializeMappingItems(),
                project = new { id = SendItem.Project.Id },
                updated = DateTime.UtcNow
            };

            var variables = new Dictionary<string, object>();
            // if id is set, we are updating an existing item
            if (SendItem.Id > 0)
            {
                variables.Add("id", SendItem.Id);
            }
            variables.Add("input", input);

            return variables;
        }
    }
}
