using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects.Buildups;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class BuildupStorageDriver : IDriverGetMany<Buildup>
    {
        public string QueryGetMany { get; } = @"
            query GetBuildups {
                calc_builder_buildups {
                    id
                    name
                    standard
                    buildup_unit
                    group {
                        group_name
                    }
                    description
                    calculation_components {
                        id
                        function
                        quantity
                        gwp
                        ge
                        calc_builder_materials_id {
                        id
                        name
                        material_category
                        standard
                        thickness
                        valid_from
                        valid_until
                        density
                        material_unit
                        gwp
                        ge
                        cost
                        }
                    }
                }
            }";

        [JsonProperty("calc_builder_buildups")]
        public List<Buildup> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
