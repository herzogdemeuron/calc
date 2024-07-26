using Calc.Core.Objects.Materials;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class MaterialFunctionStorageDriver : IDriverGetMany<MaterialFunction>
    {
        public string QueryGetMany { get; } = @"
            query GetMaterialFunctions {
                calc_material_functions {
                    id
                    name
                    amortization
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
