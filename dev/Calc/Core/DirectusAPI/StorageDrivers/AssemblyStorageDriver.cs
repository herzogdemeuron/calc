using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class AssemblyStorageDriver : IDriverGetMany<Assembly>, IDriverCreateSingle<Assembly>, IDriverUpdateSingle<Assembly>
    {
        public Assembly SendItem { get; set; }
        public string QueryGetMany { get; } = @"
            query GetAssemblies {

                calc_assemblies {

                    id
                    name
                    code
                    standards {
                        calc_standards_id {
                        id
                        name
                        }
                    }
                    assembly_unit
                    group {
                        id
                        name
                        }
                    description
                    verified
                    image {
                        id
                        }
                    speckle_project_id
                    speckle_model_id  
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
            mutation CreateBuildup($input: create_calc_assemblies_input!) {
              create_calc_assemblies_item(data: $input) {
                id
              }
            }";


        // this is a sample query, should be adjusted
        public string QueryUpdateSingle { get; } = @"
            mutation UpdateBuildup($id: ID!, $input: update_calc_assemblies_input!) {
              update_calc_assemblies_item(id: $id, data: $input) {
                id
              }
            }";


        [JsonProperty("calc_assemblies")]
        public List<Assembly> GotManyItems { get; set; }
        [JsonProperty("create_calc_assemblies_item")]
        public Assembly CreatedItem { get; set; }
        [JsonProperty("update_calc_assemblies_item")]
        public Assembly UpdatedItem { get; set; }

        /// <summary>
        /// assign materials from the store to the calculation components with their ids
        /// to simplify the query structure
        /// </summary>
        /// <param name="materials"></param>
        public void LinkMaterials(List<Material> materials)
        {
            foreach (var assembly in GotManyItems)
            {
                foreach (var calculationComponent in assembly.CalculationComponents)
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
                assembly_unit = SendItem.BuildupUnit,
                group = new { id = SendItem.Group.Id },
                description = SendItem.Description,

                standards = SendItem.StandardItems.Select(
                    s => new
                    {
                        calc_standards_id = new { id = s.Standard.Id }
                    }
                    ).ToArray(),          

                calculation_components = SendItem.CalculationComponents.Select(
                    cc => new
                    {
                        position = cc.Position,
                        function = cc.Function,
                        amount = cc.Amount,
                        carbon_a1a3 = cc.Gwp ?? 0,
                        grey_energy_fabrication_total = cc.Ge ?? 0,
                        calc_materials_id = new { id = cc.Material.Id }
                    }
                    ).ToArray(),

                carbon_a1a3 = SendItem.BuildupGwp,
                grey_energy_fabrication_total = SendItem.BuildupGe,
                assembly_snapshot = JsonConvert.SerializeObject(SendItem.BuildupSnapshot),
                speckle_model_id = SendItem.SpeckleModelId,
                speckle_project_id = SendItem.SpeckleProjectId
            };

            var inputDict = input.GetType()
                        .GetProperties()
                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(input, null));

            if (SendItem.BuildupImage != null)
            {
                inputDict.Add("image", new
                {
                    id = SendItem.BuildupImage.Id,
                    storage = "cloud",
                    filename_download = $"{SendItem.Name}.png"
                });
            }

            var variables = new Dictionary<string, object>();

            if (SendItem.Id > 0)
            {
                variables.Add("id", SendItem.Id);
            }
            variables.Add("input", inputDict);

            return variables;
        }
    }
}
