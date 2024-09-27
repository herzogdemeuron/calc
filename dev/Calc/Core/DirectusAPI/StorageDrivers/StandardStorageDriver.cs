using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Mappings;
using System.Linq;
using Calc.Core.Objects.Standards;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class StandardStorageDriver : IDriverGetMany<LcaStandard>
    {
        public string QueryGetMany { get; } = @"
            query GetStandards {
                calc_standards {
                    id
                    name
                }
            }";


        [JsonProperty("calc_standards")]
        public List<LcaStandard> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
