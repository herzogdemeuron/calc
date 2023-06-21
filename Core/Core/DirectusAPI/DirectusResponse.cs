using System;
using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;

namespace Calc.Core.DirectusAPI
{
    /// <summary>
    /// This class is used to deserialize the response from Directus.
    /// It works as the single entry point for all responses.
    /// To add a new response, add a new property to this class.
    /// To access the property, use the following syntax:
    /// <code>
    /// var directus = new Directus();
    /// var response = await directus.Client.SendMutationAsync<CollectionResponse>(request);
    /// return response.Data.Buildups;
    /// </code>
    /// Note that in the GrapqlHttpClient, the response is automatically wrapped in a Data property
    /// since this is the standard structure of a Graphql response.
    /// </summary>
    public class CollectionResponse
    {
        [JsonProperty("lca_buildups")]
        public List<Buildup> Buildups { get; set; }
        [JsonProperty("create_lca_snapshots_item")]
        public IdResponse CreateSnapshotResponse { get; set; }
        [JsonProperty("create_lca_calculation_results_items")]
        public List<IdResponse> CreateCalculationResultsResponse { get; set; }
        [JsonProperty("lca_mappings")]
        public List<Mapping> Mappings { get; set; }
        [JsonProperty("create_lca_mappings_item")]
        public IdResponse CreateMappingResponse { get; set; }
        [JsonProperty("update_lca_mappings_item")]
        public IdResponse UpdateMappingResponse { get; set; }
        [JsonProperty("create_lca_forests_item")]
        public IdResponse CreateTreeSetResponse { get; set; }
        [JsonProperty("update_lca_forests_item")]
        public IdResponse UpdateTreeSetResponse { get; set; }
        [JsonProperty("lca_forests")]
        public List<Forest> Forests { get; set; }
    }

    public class IdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
