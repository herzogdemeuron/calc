using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Mappings;
using System.Linq;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class MaterialFunctionStorageDriver : IDriverGetMany<MaterialFunction>
    {
        public string QueryGetMany { get; } = @"
            query GetMaterialFunctions {
                calc_material_functions {
                    id
                    name
                }
            }";


        [JsonProperty("calc_material_functions")]
        public List<MaterialFunction> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
