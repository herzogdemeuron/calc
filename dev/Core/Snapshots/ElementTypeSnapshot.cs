﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// The snapshot for one element type with each meta data and the material layers.
    /// Refer to calc schema.
    /// </summary>
    public class ElementTypeSnapshot
    {
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; }
        [JsonProperty("element_amount")]
        public double? ElementAmount { get; set; } // in the assembly unit
        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; } = new List<MaterialSnapshot>();
        [JsonIgnore]
        public double? TotalGwp => MaterialSnapshots.Sum(m => m.CalculatedGwp);
        [JsonIgnore]
        public double? TotalGe => MaterialSnapshots.Sum(m => m.CalculatedGe);

        public ElementTypeSnapshot(string elementTypeId)
        {
            ElementTypeId = elementTypeId;            
        }

        /// <summary>
        /// Initializes the material snapshots with an item.
        /// </summary>
        internal void AssignMaterialSnapshot(MaterialSnapshot mSnapshot)
        {
            MaterialSnapshots = [mSnapshot];
        }

        /// <summary>
        /// If two element type snapshots should be equally categirized.
        /// </summary>
        internal bool Equals(ElementTypeSnapshot other)
        {
            return ElementTypeId == other.ElementTypeId;
        }

        /// <summary>
        /// Merges another element type snapshot to this,
        /// the equality should be already ensured.
        /// </summary>
        internal void Merge(ElementTypeSnapshot other)
        {
            if (other.ElementIds != null)
            {
                ElementIds = ElementIds == null ? other.ElementIds : ElementIds.Union(other.ElementIds).ToList();
            }

            if (other.ElementAmount != null)
            {
                ElementAmount = ElementAmount == null ? other.ElementAmount : ElementAmount + other.ElementAmount;
            }

            foreach (var mSnapshot in other.MaterialSnapshots)
            {
                var existingSnapshot = MaterialSnapshots.FirstOrDefault(m => m.Equals(mSnapshot));

                if (existingSnapshot != null)
                {
                    existingSnapshot.Merge(mSnapshot);
                }
                else
                {
                    MaterialSnapshots.Add(mSnapshot.Copy());
                }
            }
        }

        internal ElementTypeSnapshot Copy()
        {
            return new ElementTypeSnapshot(ElementTypeId)
            {
                ElementIds = ElementIds,
                ElementAmount = ElementAmount,
                MaterialSnapshots = MaterialSnapshots.ConvertAll(m => m.Copy())
            };
            
        }
    }
}
