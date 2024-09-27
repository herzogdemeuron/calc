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
        [JsonProperty("element_types")]
        public List<ElementTypeSnapshot> ElementTypes { get; set; } = new List<ElementTypeSnapshot>();
        [JsonIgnore]
        public double? TotalGwp => ElementTypes.Sum(m => m.TotalGwp);
        public double? TotalGe => ElementTypes.Sum(m => m.TotalGe);

        /// <summary>
        /// claim the snapshot for one element, manipulate the the element amount and the material snapshots
        /// element ids should be later merged
        /// </summary>
        public void ClaimElement(CalcElement element, string elementGroup)
        {
            ElementGroup = elementGroup;
            var snapshot = FindElementTypeSnapshot();
            snapshot.ElementAmount = element.GetBasicUnitParameter(AssemblyUnit).Amount??0;
            snapshot.ElementIds = new List<string> { element.Id };
            foreach (var material in snapshot.MaterialSnapshots)
            {
                material.ApplyAmountRatio(snapshot.ElementAmount); // the element amount ratio equals element amount
            }
        }

        /// <summary>
        /// add a material snapshot to the corresponding element type snapshot
        /// </summary>
        public void AddMaterialSnapshot(MaterialSnapshot mSnapshot, string elementTypeId)
        {
            var snapshot = FindElementTypeSnapshot(elementTypeId);
            snapshot.MaterialSnapshots.Add(mSnapshot);
        }


        /// <summary>
        /// get the element type snapshot from the list, create a new one if not found
        /// </summary>
        private ElementTypeSnapshot FindElementTypeSnapshot(string elementTypeId=null)
        {
            var snapshot = ElementTypes.FirstOrDefault(m => m.ElementTypeId == elementTypeId);
            if (snapshot == null)
            {
                snapshot = new ElementTypeSnapshot() { ElementTypeId = elementTypeId };
                ElementTypes.Add(snapshot);
            }
            return snapshot;
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
                ElementTypes = ElementTypes.Select(s => s.Copy()).ToList()
            };
        }

    }
}
