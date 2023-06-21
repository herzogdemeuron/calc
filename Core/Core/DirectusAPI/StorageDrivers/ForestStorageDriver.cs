using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    public class ForestStorageDriver
    {
        private readonly Directus directus;

        public ForestStorageDriver(Directus directus)
        {
            this.directus = directus;
        }

        /// <summary>
        /// Saves a list of trees to the database.
        /// Specific information about the SubBranches is not saved.
        /// Instead the BranchConfig is saved wich can be used to recreate the SubBranches.
        /// </summary>
        /// <param name="forest">The forest to save.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IdResponse> SaveForestToDirectus(Forest forest)
        {
            var request = new GraphQLRequest
            {
                Query = @"
                    mutation ($forestName: String!, $trees: JSON!, $projectInput: create_architecture_projects_input) {
                        create_lca_forests_item(data: {forest_name: $forestName, trees: $trees, project_id: $projectInput}) {
                            id
                        }   
                    }",
                Variables = CreateVariables(forest)
            };

            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            if (response.Errors != null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
            return response.Data.CreateTreeSetResponse;
        }

        public async Task<IdResponse> UpdateForestInDirectus(Forest forest)
        {
            if (forest.Id < 0)
            {
                throw new Exception("forest.Id must be greater than 0");
            }

            var variables = CreateVariables(forest);

            var request = new GraphQLRequest
            {
                Query = @"
                    mutation ($forestName: String!, $trees: JSON!, $projectInput: update_architecture_projects_input, $forestId: ID!) {
                        update_lca_forests_item(id: $forestId, data: {forest_name: $forestName, trees: $trees, project_id: $projectInput}) {
                            id
                        }   
                    }",
                Variables = variables
            };

            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            if (response.Errors != null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
            return response.Data.UpdateTreeSetResponse;
        }

        public async Task<List<Forest>> GetAllForestsFromDirectus()
        {
            var request = new GraphQLRequest
            {
                Query = @"
                    query GetAllForests {
                        lca_forests {
                        id
                        forest_name
                        trees
                        project_id {
                            id
                            project_number
                        }
                        }
                    }"
            };

            var response = await directus.Client.SendQueryAsync<CollectionResponse>(request);

            if (response.Errors != null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response.Data.Forests;
        }

        private static Dictionary<string, object> CreateVariables(Forest forest)
        {
            var variables = new Dictionary<string, object>
            {
                { "forestName", forest.Name },
                { "trees", forest.SerializeTrees() },
            };

            if (forest.Project.Id > 0)
            {
                variables["projectInput"] = new { id = forest.Project.Id };
            }

            if (forest.Id > 0)
            {
                variables["forestId"] = forest.Id;
            }
            return variables;
        }
    }
}
