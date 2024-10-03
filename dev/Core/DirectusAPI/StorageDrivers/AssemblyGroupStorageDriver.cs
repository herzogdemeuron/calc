using Calc.Core.Objects.Assemblies;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get assembly groups from directus.
    /// </summary>
    internal class AssemblyGroupStorageDriver : IDriverGetMany<AssemblyGroup>
    {
        public string QueryGetMany { get; } = @"
            query GetAssemblyGroups {
                calc_assembly_groups {
                    id
                    name
                }
            }";

        [JsonProperty("calc_assembly_groups")]
        public List<AssemblyGroup> GotManyItems { get; set; }
    }
}
