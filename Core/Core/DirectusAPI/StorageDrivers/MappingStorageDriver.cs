using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    public class MappingStorageDriver
    {
        private readonly Directus directus;
        public MappingStorageDriver()
        {
            directus = new Directus();
        }

        public async Task<IdResponse> SaveMappingToDirectus(Mapping mapping)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                mutation ($mappingName: String!, $mappings: JSON!, $projectInput: create_architecture_projects_input) {
                    create_lca_mappings_item(data: {mapping_name: $mappingName, mappings: $mappings, project_id: $projectInput}) {
                        id
                    }
                }",
                Variables = CreateVariables(mapping)
            };

            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            if (response.Errors != null && response.Errors.Any())
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
            return response.Data.CreateMappingResponse;

        }

        /// <summary>
        /// WARINING: this method will overwrite an existing mapping if a mapping.Id > 0 is provided!
        /// If a mapping.Project.Id > 0 is provided, it is assumed that the project exists in Directus.
        /// </summary>
        public async Task<IdResponse> UpdateMappingInDirectus(Mapping mapping)
        {
            if (mapping.Id < 0)
            {
                throw new Exception("mappingIdInDatabase must be greater than 0");
            }

            var request = new GraphQLRequest
            {
                Query = @"
                mutation ($id: ID!, $mappingName: String!, $mappings: JSON!, $projectInput: update_architecture_projects_input) {
                    update_lca_mappings_item(id: $id, data: {
                        mapping_name: $mappingName,
                        mappings: $mappings,
                        project_id: $projectInput
                        }) {
                        id
                    }
                }",
                Variables = CreateVariables(mapping)
            };

            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            if (response.Errors != null && response.Errors.Any())
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
            return response.Data.UpdateMappingResponse;
        }

        public async Task<List<Mapping>> GetAllMappingsFromDirectus()
        {
            var request = new GraphQLRequest
            {
                Query = @"
                    query GetAllMappings {
                        lca_mappings {
                            id
                            mapping_name
                            mappings
                        }
                    }"
            };

            var response = await directus.Client.SendQueryAsync<CollectionResponse>(request);

            if (response.Errors != null && response.Errors.Any())
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response.Data.Mappings;
        }

        private static Dictionary<string, object> CreateVariables(Mapping mapping)
        {
            var variables = new Dictionary<string, object>
            {
                { "mappingName", mapping.Name },
                { "mappings", mapping.SerializeMappingItems() }
            };

            if (mapping.Project != null && mapping.Project.Id > 0)
            {
                variables.Add("projectInput", new { id = mapping.Project.Id });
            }

            if (mapping.Id > 0)
            {
                variables.Add("id", mapping.Id);
            }

            return variables;
        }
    }
}
