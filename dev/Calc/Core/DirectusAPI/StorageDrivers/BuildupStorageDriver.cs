using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Mappings;
using System.Linq;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class BuildupStorageDriver : IDriverGetMany<Buildup>, IDriverCreateSingle<Buildup>, IDriverUpdateSingle<Buildup>
    {
        public Buildup SendItem { get; set; }
        public string QueryGetMany { get; } = @"
            query GetBuildups {

                calc_buildups {

                    id
                    name
                    code
                    standards {
                        calc_standards_id {
                        id
                        name
                        }
                    }
                    buildup_unit
                    group {
                        id
                        name
                        }
                    description
                    image {
                        id
                        }
                    calculation_components {
                        id
                        position
                        function {
                            id
                            name
                        }
                        amount
                        carbon_a1a3
                        grey_energy_fabrication_total
                        calc_materials_id {
                            id
                        }           
                    }
                    carbon_a1a3
                    grey_energy_fabrication_total

                }

            }";

        // this is a sample query, should be adjusted
        public string QueryCreateSingle { get; } = @"
            mutation CreateBuildup($input: create_calc_buildups_input!) {
              create_calc_buildups_item(data: $input) {
                id
              }
            }";


        // this is a sample query, should be adjusted
        public string QueryUpdateSingle { get; } = @"
            mutation UpdateBuildup($id: ID!, $input: update_calc_buildups_input!) {
              update_calc_buildups_item(id: $id, data: $input) {
                id
              }
            }";


        [JsonProperty("calc_buildups")]
        public List<Buildup> GotManyItems { get; set; }
        [JsonProperty("create_calc_buildups_item")]
        public Buildup CreatedItem { get; set; }
        [JsonProperty("update_calc_buildups_item")]
        public Buildup UpdatedItem { get; set; }

        /// <summary>
        /// assign materials from the store to the calculation components with their ids
        /// to simplify the query structure
        /// </summary>
        /// <param name="materials"></param>
        public void LinkMaterials(List<Material> materials)
        {
            foreach (var buildup in GotManyItems)
            {
                foreach (var calculationComponent in buildup.CalculationComponents)
                {
                    calculationComponent.LinkMaterial(materials);
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
                code = SendItem.Code,
                buildup_unit = SendItem.BuildupUnit,
                group = new { id = SendItem.Group.Id },
                description = SendItem.Description,

                standards = SendItem.StandardItems.Select(
                    s => new
                    {
                        calc_standards_id = new { id = s.Standard.Id }
                    }
                    ).ToArray(),

                image = new
                { 
                    id = SendItem.BuildupImage?.Id??null,  // fix null image bug
                    storage = "cloud",
                    filename_download = $"{SendItem.Name}.png"
                },

                calculation_components = SendItem.CalculationComponents.Select(
                    cc => new
                    {
                        position = cc.Position,
                        function = cc.Function,
                        amount = cc.Amount ?? 0,
                        carbon_a1a3 = cc.Gwp ?? 0,
                        grey_energy_fabrication_total = cc.Ge ?? 0,
                        calc_materials_id = new { id = cc.Material.Id }
                    }
                    ).ToArray(),

                carbon_a1a3 = SendItem.BuildupGwp,
                grey_energy_fabrication_total = SendItem.BuildupGe,
                layer_snapshot = JsonConvert.SerializeObject(SendItem.LayerSnapshot)
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
