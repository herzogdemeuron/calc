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
        public string ElementGroup { get; set; } // query
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
        [JsonIgnore]
        public double? TotalGe => ElementTypes.Sum(m => m.TotalGe);

        /// <summary>
        /// claim the snapshot for one element, modify the the element amount and the material snapshots
        /// element ids should be later merged
        /// </summary>
        public void ClaimElement(CalcElement element, string elementGroup)
        {
            ElementGroup = elementGroup;
            var snapshot = ElementTypes.FirstOrDefault();
            if (snapshot == null) return;

            var eAmount = element.GetBasicUnitParameter(AssemblyUnit).Amount??0;
            snapshot.ElementAmount = eAmount;
            snapshot.ElementIds = new List<string> { element.Id };
            foreach (var material in snapshot.MaterialSnapshots)
            {
                material.ApplyAmountRatio(eAmount); // the element amount ratio equals element amount
            }
        }

        /// <summary>
        /// assign a material snapshot to a new element type snapshot
        /// </summary>
        public void AssignMaterialSnapshot(MaterialSnapshot mSnapshot, string elementTypeId)
        {
            var etSnapshot = new ElementTypeSnapshot(elementTypeId);
            etSnapshot.AssignMaterialSnapshot(mSnapshot);
            ElementTypes.Add(etSnapshot);
        }

        public bool Equals(AssemblySnapshot other)
        {
            return ElementGroup == other.ElementGroup &&
                   AssemblyCode == other.AssemblyCode &&
                   AssemblyGroup == other.AssemblyGroup;
        }

        /// <summary>
        /// merge another assembly snapshot to this
        /// the equality should be already ensured
        /// </summary>
        public void Merge(AssemblySnapshot other)
        {
            foreach (var etSnapshot in other.ElementTypes)
            {
                var existingSnapshot = ElementTypes.FirstOrDefault(et => et.Equals(etSnapshot));

                if (existingSnapshot != null)
                {
                    existingSnapshot.Merge(etSnapshot);
                }
                else
                {
                    ElementTypes.Add(etSnapshot.Copy());
                }
            }
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
