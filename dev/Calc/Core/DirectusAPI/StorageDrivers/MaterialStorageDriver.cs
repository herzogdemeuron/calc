using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class MaterialStorageDriver : IDriverGetMany<Material>
    {
        public string QueryGetMany { get; } = @"
            query GetMaterials {
                calc_builder_materials {
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
            }";

        [JsonProperty("calc_builder_materials")]
        public List<Material> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
