using System;
using System.Linq;
using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class SnapshotStorageDriver : IDriverCreateSingle<Snapshot>
    {
        public Snapshot SendItem { get; set; }

        public string QueryCreateSingle { get; } = @"
                    mutation ($name: String!, $results: JSON!, $projectInput: create_calc_architecture_projects_input) {
                        create_calc_snapshots_item(data: {name: $name, results: $results, project_id: $projectInput}) {
                            id
                        }   
                    }";

        [JsonProperty("create_calc_snapshots_item")]
        public Snapshot CreatedItem { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }

            var variables = new Dictionary<string, object>
            {
                { "name", this.SendItem.Name },
                { "results", JsonConvert.SerializeObject(this.SendItem.Results) }
            };

            if (this.SendItem.Project.Id >= 0)
            {
                variables["projectInput"] = new { id = this.SendItem.Project.Id };
            }

            return variables;
        }
    }
}
