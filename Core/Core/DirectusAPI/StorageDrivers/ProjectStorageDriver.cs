using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Speckle.Newtonsoft.Json;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class ProjectStorageDriver : IDriverGetMany<Project>
    {
        public string QueryGetMany { get; } = @"
                query GetProjects { 
                    calc_architecture_projects { 
                        id
                        project_number
                    }
                }";

        [JsonProperty("calc_architecture_projects")]
        public List<Project> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>();
        }
    }
}
