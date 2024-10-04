using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Calc.Core.Snapshots;

namespace Calc.Core.DirectusAPI.StorageDrivers
{
    /// <summary>
    /// Provides query patterns for the DirectusDriver, to create project result from directus.
    /// </summary>
    public class ProjectResultDriver : IDriverCreateSingle<ProjectResult>
    {
        public ProjectResult SendItem { get; set; }

        public string QueryCreateSingle { get; } = @"
            mutation CreateSnapshot($input: create_calc_snapshots_input!) {
              create_calc_snapshots_item(data: $input) {
                id
              }
            }";


        [JsonProperty("create_calc_snapshots_item")]
        public ProjectResult CreatedItem { get; set; }

        /// <summary>
        /// Provides creation variables.
        /// </summary>
        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }

            var input = new
            {
                name = SendItem.Name,
                project = new { id = SendItem.Project.Id },
                snapshot_file = new
                {
                    id = SendItem.JsonUuid,
                    storage = "cloud",
                    filename_download = $"{SendItem.Name}.json",
                    description = SendItem.Description
                },
                description = SendItem.Description
            };

            var variables = new Dictionary<string, object>
            {
                { "input", input }
            };
            return variables;
        }
    }
}
