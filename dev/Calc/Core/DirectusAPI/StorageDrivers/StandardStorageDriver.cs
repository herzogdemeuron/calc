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
    public class StandardStorageDriver : IDriverGetMany<Standard>
    {
        public string QueryGetMany { get; } = @"
            query GetStandards {
                calc_standards {
                    id
                    name
                }
            }";


        [JsonProperty("calc_standards")]
        public List<Standard> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
