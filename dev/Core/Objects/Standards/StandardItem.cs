using Newtonsoft.Json;

namespace Calc.Core.Objects.Standards
{
    /// <summary>
    /// This class is exclusively used to map the junction item, from many to many relations: assembly - standard
    /// each standard item contains one LcaStandard that are related to an assembly 
    /// </summary>
    public class StandardItem
    {
        [JsonProperty("calc_standards_id")]
        public LcaStandard Standard { get; set; }
    }
}
