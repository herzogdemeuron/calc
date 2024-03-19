using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class Material : ISearchable
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("standard")]
        public LcaStandard Standard { get; set; }
        [JsonProperty("material_category")]
        public string Category { get; set; }
        [JsonProperty("thickness")]
        public double? Thickness { get; set; }

        // deprecated
/*        [JsonProperty("density")]
        public double? Density { get; set; }*/

        [JsonProperty("material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty("valid_from")]
        public int? ValidFrom { get; set; }
        [JsonProperty("valid_until")]
        public int? ValidUntil { get; set; }
        [JsonProperty("gwp")]
        public double? Gwp { get; set; }

        [JsonProperty("ge")]
        public double? Ge { get; set; }

        [JsonProperty("cost")]
        public double? Cost { get; set; }

        public string GroupName => Category;
        //public string SourceCode { get; set; }

        public void LinkStandard(List<LcaStandard> standards)
        {
            if (Standard != null)
            {
                Standard = standards.Find(s => s.Id == Standard.Id);
            }
        }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, Category: {Category}";
        }
    }

}
