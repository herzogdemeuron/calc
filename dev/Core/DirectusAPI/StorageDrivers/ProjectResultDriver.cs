using Calc.Core.Snapshots;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

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
        [JsonIgnore]
        public string StorageType { get; set; }

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
                    storage = StorageType,
                    filename_download = $"{SendItem.Name}.json",
                    description = SendItem.Description
                },
                description = SendItem.Description,
                snapshot_summary = JsonConvert.SerializeObject(SendItem.SnapshotSummary)
            };

            var variables = new Dictionary<string, object>
            {
                { "input", input }
            };
            return variables;
        }
    }
}
