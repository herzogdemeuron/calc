using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.Drivers
{
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
                        stages
                        impact_categories
                    }
                }";

        [JsonProperty("calc_architecture_projects")]
        public List<CalcProject> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
