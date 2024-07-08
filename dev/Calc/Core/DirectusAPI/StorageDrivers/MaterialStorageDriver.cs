using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class MaterialStorageDriver : IDriverGetMany<Material>
    {
        public string QueryGetMany { get; } = @"
            query GetMaterials {
                calc_materials(limit: 5000) {
                    id
                    name
                    material_type_family
                    material_type
                    product_type_family
                    product_type
                    standard {
                        id
                        name
                    }
                    data_source
                    source_uuid
                    thickness
                    updated
                    material_unit
                    carbon_a1a3
                    grey_energy_fabrication_total
                    cost
                }
            }";

        [JsonProperty("calc_materials")]
        public List<Material> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
