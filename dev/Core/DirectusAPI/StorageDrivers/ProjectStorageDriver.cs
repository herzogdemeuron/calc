using Calc.Core.Objects;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to get projects from directus.
    /// </summary>
    public class ProjectStorageDriver : IDriverGetMany<CalcProject>
    {
        public string QueryGetMany { get; } = @"
                query GetProjects { 
                    calc_architecture_projects { 
                        id
                        number
                        name
                        location
                        lca_method
                        life_span
                        area
                        stages
                        impact_categories
                    }
                }";

        [JsonProperty("calc_architecture_projects")]
        public List<CalcProject> GotManyItems { get; set; }
    }
}
