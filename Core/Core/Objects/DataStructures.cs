using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speckle.Newtonsoft.Json;
using Calc.Core.DirectusAPI;

namespace Calc.Core.Objects
{
    public enum Unit
    {
        each,
        m,
        m2,
        m3
    }

    public readonly struct CalcElement
    {
        public readonly string Id;
        public readonly decimal Length;
        public readonly decimal Area;
        public readonly decimal Volume;
        public readonly Dictionary<string, object> Fields;

        public CalcElement(string id,
            Dictionary<string, object> fields,
            decimal length=0,
            decimal area=0,
            decimal volume=0)
        {
            this.Id = id;
            this.Length = length;
            this.Area = area;
            this.Volume = volume;
            this.Fields = fields;
        }
    }

    public class Project
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "project_number")]
        public string ProjectNumber { get; set; }
    }

    public class Result : IHasProject
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "snapshot_name")]
        public string SnapshotName { get; set; }
        [JsonProperty(PropertyName = "element_id")]
        public string ElementId { get; set; }
        [JsonProperty(PropertyName = "global_warming_potential_a1_a2_a3")]
        public decimal GlobalWarmingPotentialA1A2A3 { get; set; }
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }
        [JsonProperty(PropertyName = "material_amount")]
        public decimal MaterialAmount { get; set; }
        [JsonProperty(PropertyName = "material_name")]
        public string MaterialName { get; set; }
        [JsonProperty(PropertyName = "material_category")]
        public string MaterialCategory { get; set; }
        [JsonProperty(PropertyName = "buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty(PropertyName = "group_name")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "project_id", NullValueHandling = NullValueHandling.Ignore)]
        public Project Project { get; set; }
    }

    public class Material
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get;  set; }
        [JsonProperty("material_name")]
        public string Name { get;  set; }
        [JsonProperty("global_warming_potential_a1_a2_a3")]
        public decimal GwpA123 { get;  set; }
        [JsonProperty("unit")]
        public string Unit { get;  set; }
        [JsonProperty("material_category")]
        public string Category { get;  set; }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, GWP: {GwpA123}, Category: {Category}";
        }
    }

    public class BuildupComponent
    {
        [JsonProperty("calc_materials_id")]
        public Material Material { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return $"Amount: {Amount}";
        }
    }

    public class Buildup
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty("buildup_name")]
        public string Name { get; set; }
        [JsonProperty("group_id")]
        public MaterialGroup Group { get; set; }
        [JsonProperty("components")]
        public List<BuildupComponent> Components { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public class MaterialGroup
    {
        [JsonProperty("group_name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Group Name: {Name}";
        }
    }
}
