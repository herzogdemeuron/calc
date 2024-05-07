using System;
using System.Linq;
using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects.Results;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class SnapshotStorageDriver : IDriverCreateSingle<Snapshot>
    {
        public Snapshot SendItem { get; set; }

        public string QueryCreateSingle { get; } = @"
            mutation CreateSnapshot($input: create_calc_snapshots_input!) {
              create_calc_snapshots_item(data: $input) {
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

            var input = new
            {
                name = SendItem.Name,
                project = new { id = SendItem.Project.Id },
                results = JsonConvert.SerializeObject(SendItem.Results)
            };

            var variables = new Dictionary<string, object>();
            variables.Add("input", input);
            return variables;
        }
    }
}
