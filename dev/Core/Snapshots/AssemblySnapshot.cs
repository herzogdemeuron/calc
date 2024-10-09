using Calc.Core.Objects;
using Calc.Core.Objects.Elements;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// The snapshot for an assembly, serialized to directus.
    /// Refer to calc schema.
    /// </summary>
    public class AssemblySnapshot
    {
        [JsonProperty("query_name")]
        public string QueryName { get; set; }
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
        /// Claims the snapshot for one element, modifies the the element amount and the material snapshots.
        /// Element ids should be later merged.
        /// </summary>
        internal void ClaimElement(CalcElement element, string elementGroup)
        {
            QueryName = elementGroup;
            var snapshot = ElementTypes.FirstOrDefault();
            if (snapshot == null) return;

            var eAmount = element.GetBasicUnitParameter(AssemblyUnit).Amount??0;
            snapshot.ElementAmount = eAmount;
            snapshot.ElementIds = new List<string> { element.Id };
            foreach (var material in snapshot.MaterialSnapshots)
            {
                // the element amount ratio equals element amount
                material.ApplyAmountRatio(eAmount); 
            }
        }

        /// <summary>
        /// Assigns a material snapshot to a new element type snapshot
        /// </summary>
        internal void AssignMaterialSnapshot(MaterialSnapshot mSnapshot, string elementTypeId)
        {
            var etSnapshot = new ElementTypeSnapshot(elementTypeId);
            etSnapshot.AssignMaterialSnapshot(mSnapshot);
            ElementTypes.Add(etSnapshot);
        }

        /// <summary>
        /// If two assembly snapshots should be equally categirized.
        /// </summary>
        internal bool Equals(AssemblySnapshot other)
        {
            return QueryName == other.QueryName &&
                   AssemblyCode == other.AssemblyCode &&
                   AssemblyGroup == other.AssemblyGroup;
        }

        /// <summary>
        /// Merges another assembly snapshot to this,
        /// the equality should be already ensured.
        /// </summary>
        internal void Merge(AssemblySnapshot other)
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
                
        internal AssemblySnapshot Copy()
        {
            return new AssemblySnapshot
            {
                QueryName = QueryName,
                AssemblyName = AssemblyName,
                AssemblyCode = AssemblyCode,
                AssemblyGroup = AssemblyGroup,
                AssemblyUnit = AssemblyUnit,
                ElementTypes = ElementTypes.Select(s => s.Copy()).ToList()
            };
        }

    }
}
