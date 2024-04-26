using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
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
                    material_type
                    product_type
                    product_type
                    standard {
                        id
                    }
                    thickness
                    updated
                    material_unit
                    carbon_a1a3
                    grey_energy_fabrication_total
                    cost
                }
            }";

        public void LinkStandards(List<LcaStandard> standards)
        {
            foreach (var material in GotManyItems)
            {
                material.LinkStandard(standards);
            }
        }

        [JsonProperty("calc_materials")]
        public List<Material> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
