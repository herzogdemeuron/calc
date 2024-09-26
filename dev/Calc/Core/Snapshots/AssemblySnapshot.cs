using Calc.Core.Objects;
using Calc.Core.Objects.Elements;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    public class AssemblySnapshot
    {
        [JsonProperty("element_group")]
        public string ElementGroup { get; set; } // tree
        [JsonProperty("assembly_name")]
        public string AssemblyName { get; set; }
        [JsonProperty("assembly_code")]
        public string AssemblyCode { get; set; }
        [JsonProperty("assembly_group")]
        public string AssemblyGroup { get; set; }
        [JsonProperty("assembly_unit")]
        public Unit AssemblyUnit { get; set; }
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; } = new List<string>();
        [JsonProperty("element_amount")]
        public double ElementAmount { get; set; } // uses the assembly unit

        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; }
        [JsonIgnore]
        public double? TotalGwp => MaterialSnapshots.Sum(m => m.CalculatedGwp);
        public double? TotalGe => MaterialSnapshots.Sum(m => m.CalculatedGe);

        /// <summary>
        /// claim the snapshot for the element, manipulate the the element amount and the material snapshots
        /// </summary>
        public void ClaimElement(CalcElement element, string elementGroup)
        {
            ElementAmount = element.GetBasicUnitParameter(AssemblyUnit).Amount??0;
            ElementIds = new List<string> { element.Id };
            ElementGroup = elementGroup;
            foreach (var material in MaterialSnapshots)
            {
                material.ApplyAmountRatio(ElementAmount); // the element amount ratio equals element amount
            }
        }

        public void ClaimElementTypeId(string elementTypeId)
        {
            ElementTypeId = elementTypeId;
        }

        public AssemblySnapshot Copy()
        {
            return new AssemblySnapshot
            {
                ElementGroup = ElementGroup,
                AssemblyName = AssemblyName,
                AssemblyCode = AssemblyCode,
                AssemblyGroup = AssemblyGroup,
                AssemblyUnit = AssemblyUnit,
                ElementTypeId = ElementTypeId,
                ElementIds = ElementIds,
                ElementAmount = ElementAmount,
                MaterialSnapshots = MaterialSnapshots.ConvertAll(m => m.Copy())
            };
        }

    }
}
