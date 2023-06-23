using Calc.Core.Objects;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.DirectusAPI.Drivers
{
    public class ForestStorageDriver : IDriverCreateSingle<Forest>, IDriverUpdateSingle<Forest>, IDriverGetMany<Forest>
    {
        public Forest SendItem { get; set; }

        public string QueryGetMany { get; } = @"
                    query GetAllForests {
                        lca_forests {
                        id
                        forest_name
                        trees
                        project_id {
                            id
                            project_number
                        }
                        }
                    }";
        public string QueryCreateSingle { get; } = @"
                    mutation ($forestName: String!, $trees: JSON!, $projectInput: create_architecture_projects_input) {
                        create_lca_forests_item(data: {forest_name: $forestName, trees: $trees, project_id: $projectInput}) {
                            id
                        }   
                    }";
        public string QueryUpdateSingle { get; } = @"
                    mutation ($forestName: String!, $trees: JSON!, $projectInput: update_architecture_projects_input, $forestId: ID!) {
                        update_lca_forests_item(id: $forestId, data: {forest_name: $forestName, trees: $trees, project_id: $projectInput}) {
                            id
                        }   
                    }";

        [JsonProperty("lca_forests")]
        public List<Forest> GotManyItems { get; set; }
        [JsonProperty("create_lca_forests_item")]
        public Forest CreatedItem { get; set; }
        [JsonProperty("update_lca_forests_item")]
        public Forest UpdatedItem { get; set; }

        public Dictionary<string, object> GetVariables()
        {
            if (this.SendItem == null)
            {
                return new Dictionary<string, object>();
            }

            var variables = new Dictionary<string, object>
            {
                { "forestName", this.SendItem.Name },
                { "trees", this.SendItem.SerializeTrees() },
            };

            if (this.SendItem.Project.Id > 0)
            {
                variables["projectInput"] = new { id = this.SendItem.Project.Id };
            }

            if (this.SendItem.Id > 0)
            {
                variables["forestId"] = this.SendItem.Id;
            }
            return variables;
        }
    }
}
