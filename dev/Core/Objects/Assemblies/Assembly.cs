using Calc.Core.Calculation;
using Calc.Core.Objects.Standards;
using Calc.Core.Snapshots;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Objects.Assemblies
{
    public class Assembly : ISearchable
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("standards")]
        public List<StandardItem> StandardItems { get; set; }
        [JsonIgnore]
        public string StandardsString { get => StandardItems.Select(x => x.Standard.Name).Aggregate((x, y) => x + ", " + y); }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("assembly_unit")]
        public Unit AssemblyUnit { get; set; }
        [JsonProperty("group")]
        public AssemblyGroup Group { get; set; }
        public string GroupName => Group?.Name;
        [JsonProperty("carbon_a1a3")]
        public double? AssemblyGwp { get; set; }
        [JsonProperty("grey_energy_fabrication_total")]
        public double? AssemblyGe { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("image")]
        public AssemblyImage AssemblyImage { get; set; }
        [JsonProperty("speckle_project_id")]
        public string SpeckleProjectId { get; set; }
        [JsonProperty("speckle_model_id")]
        public string SpeckleModelId { get; set; }
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents { get; set; }
        [JsonIgnore]
        public AssemblySnapshot AssemblySnapshot { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Assembly other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
