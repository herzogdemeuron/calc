using System;
using System.Collections.Generic;
using Calc.Core.Objects;

namespace Calc.Core.Calculations
{
    public class GwpCalculator
    {
        /// <summary>
        /// Intended use:
        /// <code>
        /// var branches = new List<Branch>();
        /// foreach (var tree in trees)
        /// {
        ///     tree.RemoveElementsByBuildupOverrides();
        ///     branches.AddRange(tree.Trunk.Flatten());
        /// }
        /// var results = GwpCalculator.CalculateGwp(branches);
        /// </code>
        public static List<Result> CalculateGwp(List<Branch> branches)
        {
            var results = new List<Result>();
            foreach (var branch in branches)
            {
                // check if branch has buildup (if not, skip)
                if (branch.Buildup == null) continue;

                var buildup = branch.Buildup;
                foreach (var element in branch.Elements)
                {
                    foreach (var component in buildup.Components)
                    {
                        var material = component.Material;
                        var gwpA123 = CalculateGwpA123(element, component, buildup.Unit);
                        var calculationResult = new Result
                        { 
                            ElementId = element.Id,
                            GlobalWarmingPotentialA1A2A3 = gwpA123,
                            Unit = buildup.Unit,
                            MaterialAmount = component.Amount,
                            MaterialName = material.Name,
                            MaterialCategory = material.Category,
                            BuildupName = buildup.Name,
                            GroupName = buildup.Group.Name
                        };

                        results.Add(calculationResult);
                    }
                }
            }
            return results;
        }

        public static decimal CalculateGwpA123(CalcElement element, BuildupComponent component, string unit)
        {
            var material = component.Material;
            return unit switch
            {
                "each" => material.GwpA123 * component.Amount,
                "m" => material.GwpA123 * component.Amount * element.Length,
                "m2" => material.GwpA123 * component.Amount * element.Area,
                "m3" => material.GwpA123 * component.Amount * element.Volume,
                _ => throw new Exception("Unit not recognized"),
            };
        }
    }
}
