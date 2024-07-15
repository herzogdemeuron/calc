using Calc.Core.Objects;
using Calc.Core.Objects.Elements;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    public class BuildupSnapshot
    {
        [JsonProperty("element_group")]
        public string ElementGroup { get; set; } // tree
        [JsonProperty("buidup_name")]
        public string BuildupName { get; set; }
        [JsonProperty("buildup_code")]
        public string BuildupCode { get; set; }
        [JsonProperty("buildup_group")]
        public string BuildupGroup { get; set; }
        [JsonProperty("buildup_unit")]
        public Unit BuildupUnit { get; set; }
        [JsonProperty("element_type_id")]
        public string ElementTypeId { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; } = new List<string>();
        [JsonProperty("element_unit")]
        public double ElementAmount { get; set; } // uses the buildup unit

        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; }

        /// <summary>
        /// claim the snapshot for the element, manipulate the the element amount and the material snapshots
        /// </summary>
        public void ClaimElement(CalcElement element, string elementGroup)
        {
            ElementAmount = element.GetBasicUnitParameter(BuildupUnit).Amount.Value;
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

        public BuildupSnapshot Copy()
        {
            return new BuildupSnapshot
            {
                ElementGroup = ElementGroup,
                BuildupName = BuildupName,
                BuildupCode = BuildupCode,
                BuildupGroup = BuildupGroup,
                BuildupUnit = BuildupUnit,
                ElementTypeId = ElementTypeId,
                ElementIds = ElementIds,
                ElementAmount = ElementAmount,
                MaterialSnapshots = MaterialSnapshots.ConvertAll(m => m.Copy())
            };
        }

    }
}
