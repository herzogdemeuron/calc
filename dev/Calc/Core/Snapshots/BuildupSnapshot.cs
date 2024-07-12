﻿using Calc.Core.Objects;
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
        [JsonProperty("element_type_ids")]
        public List<string> ElementTypeIds { get; set; }
        [JsonProperty("element_ids")]
        public List<string> ElementIds { get; set; }
        [JsonProperty("element_unit")]
        public double ElementAmount { get; set; } // uses the buildup unit
        [JsonProperty("buildup_gwp")]
        public double? BuildupGwp { get; set; }
        [JsonProperty("buildup_ge")]
        public double? BuildupGe { get; set; }
        [JsonProperty("materials")]
        public List<MaterialSnapshot> MaterialSnapshots { get; set; }

        /// <summary>
        /// claim the snapshot for the element, manipulate the the element amount and the material snapshots
        /// </summary>
        public void ClaimElement(CalcElement element)
        {
            ElementAmount = element.GetBasicUnitParameter(BuildupUnit).Amount.Value;
            ElementIds = new List<string> { element.Id };
            BuildupGwp = BuildupGwp * ElementAmount;
            BuildupGe = BuildupGe * ElementAmount;

            foreach (var material in MaterialSnapshots)
            {
                material.ApplyRatio(ElementAmount);
            }
        }

        public void ClaimElementTypeId(string elementTypeId)
        {
            ElementTypeIds = new List<string> { elementTypeId };
        }

    }
}
