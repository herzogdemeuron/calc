using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Mappings;
using System.Linq;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class BuildupGroupStorageDriver : IDriverGetMany<BuildupGroup>
    {
        public string QueryGetMany { get; } = @"
            query GetBuildupGroups {
                calc_buildup_groups {
                    id
                    name
                }
            }";


        [JsonProperty("calc_buildup_groups")]
        public List<BuildupGroup> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
