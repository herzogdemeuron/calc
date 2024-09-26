using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Mappings;
using System.Linq;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class AssemblyGroupStorageDriver : IDriverGetMany<AssemblyGroup>
    {
        public string QueryGetMany { get; } = @"
            query GetBuildupGroups {
                calc_assembly_groups {
                    id
                    name
                }
            }";


        [JsonProperty("calc_assembly_groups")]
        public List<AssemblyGroup> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
