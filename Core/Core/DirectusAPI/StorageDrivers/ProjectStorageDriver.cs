using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class ProjectStorageDriver : IDriverGetMany<Project>
    {
        public string QueryGetMany { get; } = @"
                query GetProjects { 
                    lca_projects { 
                        id
                        project_name
                    }
                }";

        public List<Project> GotManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            throw new NotImplementedException();
        }
    }
}
