using Calc.Core.Objects.Materials;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get materials from directus.
    /// </summary>
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
    }
}
