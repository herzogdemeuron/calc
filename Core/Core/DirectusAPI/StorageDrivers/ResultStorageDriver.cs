using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    public class ResultStorageDriver
    {
        private readonly Directus directus;

        public ResultStorageDriver()
        {
            directus = new Directus();
        }

        /// <summary>
        /// Save results to Directus.
        /// </summary>
        /// <param name="results">List of CalculationResult objects</param>
        /// <param name="snapshot">[Optional] Snapshot object. If not provided, an unnamed snapshot will be created.</param>
        public static async Task<List<IdResponse>> SaveResultsToDirectus(List<CalculationResult> results, Snapshot snapshot = default)
        {
            var driver = new ResultStorageDriver();
            var createdSnapshot = await driver.CreateSnapshotItem(snapshot);
            // add snapshot id to results
            foreach (var result in results)
            {
                result.Snapshot = createdSnapshot;
            }

            return await driver.CreateResultItems(results);
        }

        private async Task<List<IdResponse>> CreateResultItems(List<CalculationResult> results)
        {
            var mutation = @"
                mutation CreateCalculationResults($data: [create_lca_calculation_results_input!]!) {
                    create_lca_calculation_results_items(data: $data) {
                        id
                    }
                }
            ";

            var variables = new
            {
                data = results.Select(result => new
                {
                    snapshot_id = new
                    {
                        id = result.Snapshot.Id
                    },
                    element_id = result.ElementId,
                    global_warming_potential_a1_a2_a3 = result.GlobalWarmingPotentialA1A2A3,
                    unit = result.Unit,
                    material_amount = result.MaterialAmount,
                    material_name = result.MaterialName,
                    material_category = result.MaterialCategory,
                    buildup_name = result.BuildupName,
                    group_name = result.GroupName
                }).ToList()
            };

            var request = new GraphQLRequest
            {
                Query = mutation,
                Variables = variables
            };


            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            if (response.Errors != null && response.Errors.Any())
            {
                throw new Exception(mutation + " " + JsonConvert.SerializeObject(variables, Formatting.Indented) + " " + JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
            return response.Data.CreateCalculationResultsResponse;
        }

        private async Task<IdResponse> CreateSnapshotItem(Snapshot snapshot = default)
        {
            // check for all kinds of conditions to ensure the query will not fail
            if (snapshot == default)
            {
                snapshot = new Snapshot
                {
                    Name = "unnamed",
                    Project = new Project
                    {
                        Id = 0,
                        ProjectNumber = ""
                    }
                };
            }
            else
            {
                if (snapshot.Project == default)
                {
                    snapshot.Project = new Project
                    {
                        Id = 0,
                        ProjectNumber = ""
                    };
                }
                else if (snapshot.Project.ProjectNumber == default)
                {
                    snapshot.Project.ProjectNumber = "";
                    snapshot.Project.Id = 0;
                }
                else if (snapshot.Project.Id == default)
                {
                    snapshot.Project.Id = 0;
                }
            }

            var request = new GraphQLRequest
            {
                Query = @"
                    mutation($snapshotName: String!, $projectId: ID!, $projectNumber: String!) {
                        create_lca_snapshots_item(data: {snapshot_name: $snapshotName, project_id: {id: $projectId, project_number: $projectNumber}}) {
                            id
                          }
                        }",
                Variables = new
                {
                    snapshotName = snapshot.Name,
                    projectId = snapshot.Project.Id,
                    projectNumber = snapshot.Project.ProjectNumber
                }
            };

            var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
            return response.Data.CreateSnapshotResponse;
        }
    }
}
