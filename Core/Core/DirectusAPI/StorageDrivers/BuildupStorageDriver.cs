using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    public class BuildupStorageDriver
    {
        private readonly Directus directus;

        public BuildupStorageDriver(Directus directus)
        {
            this.directus = directus;
        }

        public async Task<List<Buildup>> GetAllBuildupsFromDirectus()
        {
            var request = new GraphQLRequest
            {
                Query = @"
                    query GetBuildups {
                      lca_buildups {
                        id
                        buildup_name
                        group_id {
                          group_name
                        }
                        components {
                          lca_materials_id {
                            id
                            material_name
                            global_warming_potential_a1_a2_a3
                            unit
                            material_category
                          }
                          amount
                        }
                        unit
                      }
                    }"
            };
            var response = await directus.Client.SendQueryAsync<CollectionResponse>(request);
            return response.Data.Buildups;
        }
    }
}
