using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Mappings;
using System.Linq;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class BuildupStorageDriver : IDriverGetMany<Buildup>, IDriverCreateSingle<Buildup>, IDriverUpdateSingle<Buildup>
    {
        public Buildup SendItem { get; set; }
        public string QueryGetMany { get; } = @"
            query GetBuildups {
                calc_builder_buildups {
                    id
                    name
                    standard
                    buildup_unit
                    group {
                        group_name
                    }
                    description
                    calculation_components {
                        id
                        function
                        quantity
                        gwp
                        ge
                        calc_builder_materials_id {
                            id
                        }           
                    }
                }
            }";

        // this is a sample query, should be adjusted
        public string QueryCreateSingle { get; } = @"
            mutation CreateBuildup($input: create_calc_builder_buildups_input!) {
              create_calc_builder_buildups_item(data: $input) {
                id
              }
            }";


        // this is a sample query, should be adjusted
        public string QueryUpdateSingle { get; } = @"
            mutation UpdateBuildup($id: ID!, $input: update_calc_builder_buildups_input!) {
              update_calc_builder_buildups_item(id: $id, data: $input) {
                id
              }
            }";


        [JsonProperty("calc_builder_buildups")]
        public List<Buildup> GotManyItems { get; set; }
        [JsonProperty("create_calc_builder_buildups_item")]
        public Buildup CreatedItem { get; set; }
        [JsonProperty("update_calc_builder_buildups_item")]
        public Buildup UpdatedItem { get; set; }

        public void ReferenceMaterials(List<Material> materials)
        {
            foreach (var buildup in GotManyItems)
            {
                foreach (var calculationComponent in buildup.CalculationComponents)
                {
                    calculationComponent.ReferenceMaterial(materials);
                }
            }
        }
        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }
            var input = new
            {
                name = SendItem.Name,
                standard = SendItem.Source,
                buildup_unit = SendItem.BuildupUnit,
                //group = SendItem.Group?.Id ?? 0,
                description = SendItem.Description,
                calculation_components = SendItem.CalculationComponents.Select(cc => new
                {
                    function = cc.Function,
                    quantity = cc.Quantity ?? 0,
                    gwp = cc.GWP ?? 0,
                    ge = cc.GE ?? 0,
                    calc_builder_materials_id = new { id = cc.Material.Id}
                }).ToArray()
            };

            var variables = new Dictionary<string, object>();

            if (SendItem.Id > 0)
            {
                variables.Add("id", SendItem.Id);
            }
            variables.Add("input", input);

            return variables;
        }
    }
}
