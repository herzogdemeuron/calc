using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Speckle.Newtonsoft.Json;
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
                calc_builder_buildups {
                    id
                    name
                    standard {
                        id
                    }
                    buildup_unit
                    group {
                        id
                    }
                    description
                    calculation_components {
                        id
                        function
                        amount
                        gwp
                        ge
                        calc_builder_materials_id {
                            id
                        }           
                    }
                    buildup_gwp
                    buildup_ge
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
        public void LinkBuildupGroups(List<BuildupGroup> buildupGroups)
        {
            foreach (var buildup in GotManyItems)
            {
                buildup.LinkGroup(buildupGroups);
            }
        }

        public void LinkStandards(List<LcaStandard> standards)
        {
            foreach (var buildup in GotManyItems)
            {
                buildup.LinkStandard(standards);
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
                standard = SendItem.Standard,
                buildup_unit = SendItem.BuildupUnit,
                group = new { id = SendItem.Group.Id},
                description = SendItem.Description,
                calculation_components = SendItem.CalculationComponents.Select(cc => new
                {
                    position = cc.Position,
                    function = cc.Function,
                    amount = cc.Amount ?? 0,
                    gwp = cc.Gwp ?? 0,
                    ge = cc.Ge ?? 0,
                    calc_builder_materials_id = new { id = cc.Material.Id}
                }).ToArray(),
                buildup_gwp = SendItem.BuildupGwp,
                buildup_ge = SendItem.BuildupGe
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
