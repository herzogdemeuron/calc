using System.Collections.Generic;
using Calc.Core.Objects;
using Speckle.Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class MappingStorageDriver : IDriverCreateSingle<Mapping>, IDriverGetMany<Mapping>, IDriverUpdateSingle<Mapping>
    {
        public Mapping SendItem { get; set; }

        public string QueryGetMany { get; } = @"
                    query GetAllMappings {
                        calc_mappings {
                            id
                            mapping_name
                            mappings
                            project_id {
                                id
                                project_number
                            }
                        }
                    }";
        public string QueryCreateSingle { get; } = @"
                mutation ($mappingName: String!, $mappings: JSON!, $projectInput: create_calc_architecture_projects_input) {
                    create_calc_mappings_item(data: {mapping_name: $mappingName, mappings: $mappings, project_id: $projectInput}) {
                        id
                    }
                }";
        public string QueryUpdateSingle { get; } = @"
                mutation ($id: ID!, $mappingName: String!, $mappings: JSON!, $projectInput: update_calc_architecture_projects_input) {
                    update_calc_mappings_item(id: $id, data: {
                        mapping_name: $mappingName,
                        mappings: $mappings,
                        project_id: $projectInput
                        }) {
                        id
                    }
                }";

        [JsonProperty("calc_mappings")]
        public List<Mapping> GotManyItems { get; set; }
        [JsonProperty("create_calc_mappings_item")]
        public Mapping CreatedItem { get; set; }
        [JsonProperty("update_calc_mappings_item")]
        public Mapping UpdatedItem { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }

            var variables = new Dictionary<string, object>
            {
                { "mappingName", this.SendItem.Name },
                { "mappings", this.SendItem.SerializeMappingItems() }
            };

            if (this.SendItem.Project != null && this.SendItem.Project.Id > 0)
            {
                variables.Add("projectInput", new { id = this.SendItem.Project.Id });
            }

            if (this.SendItem.Id > 0)
            {
                variables.Add("id", this.SendItem.Id);
            }

            return variables;
        }
    }
}
