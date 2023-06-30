using System;
using System.Linq;
using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class ResultStorageDriver : IDriverCreateMany<Result>
    {
        public List<Result> SendItems { get; set; }

        public string QueryCreateMany { get; } = @"
                mutation CreateCalculationResults($data: [create_calc_calculation_results_input!]!) {
                    create_calc_calculation_results_items(data: $data) {
                        id
                    }
                }";

        [JsonProperty("create_calc_calculation_results_items")]
        public List<Result> CreatedManyItems { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItems == null)
            {
                return new Dictionary<string, object>();
            }
            Console.WriteLine(JsonConvert.SerializeObject(this.SendItems, Formatting.Indented));
            var variables = new Dictionary<string, object>
            {
                { "data", this.SendItems.Select(r => new Dictionary<string, object>
                    {
                        { "snapshot_name", r.SnapshotName },
                        { "element_id", r.ElementId },
                        { "global_warming_potential_a1_a2_a3", r.GlobalWarmingPotentialA1A2A3 },
                        { "unit", r.Unit },
                        { "material_amount", r.MaterialAmount },
                        { "material_name", r.MaterialName },
                        { "material_category", r.MaterialCategory },
                        { "buildup_name", r.BuildupName },
                        { "group_name", r.GroupName },
                        { "project_id", new Dictionary<string, object> { { "id", r.Project.Id } } }
                    }).ToList()
                }
            };
            return variables;
        }
    }
}
