using System;
using System.Collections.Generic;
using Calc.Core.Objects;

namespace Calc.Core.Calculations
{
    public class Calculator
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
        public static List<Result> Calculate(List<Branch> branches)
        {
            var flatBranches = new List<Branch>();
            foreach (var branch in branches)
            {
                // make a copy of the branch and remove all elements that are overridden by a buildup
                var branchCopy = branch.Copy();
                branchCopy.RemoveElementsByBuildupOverrides();
                flatBranches.AddRange(branchCopy.Flatten());
            }

            // add cost for testing ------------------------------------------
            // loop through branches, for each branch, loop through elements, for each element, loop through components
            // add a random cost to each component.Material.Cost
            // make dictionary of material id and cost
            // reuse cost if material id is already in dictionary
            //var materialCosts = new Dictionary<int, decimal>();
            //var random = new Random();
            //foreach (var branch in flatBranches)
            //{
            //    if (branch.Buildup == null) continue;

            //    var buildup = branch.Buildup;

            //    if (buildup.Components == null) continue;

            //    foreach (var element in branch.Elements)
            //    {
            //        foreach (var component in buildup.Components)
            //        {
            //            var material = component.Material;
            //            if (!materialCosts.ContainsKey(material.Id))
            //            {
            //                materialCosts.Add(material.Id, (decimal)random.NextDouble() * 100);
            //            }
            //            material.Cost = materialCosts[material.Id];
            //        }
            //    }
            //}
            // add cost for testing ------------------------------------------

            var results = new List<Result>();
            foreach (var branch in flatBranches)
            {
                if (branch.Buildup == null) continue;

                var buildup = branch.Buildup;

                if (buildup.Components == null) continue;

                foreach (var element in branch.Elements)
                {
                    foreach (var component in buildup.Components)
                    {
                        var material = component.Material;
                        var gwpA123 = CalculateGwpA123(element, component, buildup.Unit);
                        var cost = CalculateCost(element, component, buildup.Unit);
                        var calculationResult = new Result
                        { 
                            ElementId = element.Id,
                            GlobalWarmingPotentialA1A2A3 = gwpA123,
                            Cost = cost,
                            Unit = buildup.Unit,
                            MaterialAmount = component.Amount,
                            MaterialName = material.Name,
                            MaterialCategory = material.Category,
                            BuildupName = buildup.Name,
                            GroupName = buildup.Group.Name,
                            Color = branch.HslColor
                        };

                        results.Add(calculationResult);
                    }
                }
            }
            return results;
        }

        private static decimal CalculateGwpA123(CalcElement element, BuildupComponent component, Unit unit)
        {
            var material = component.Material;
            return unit switch
            {
                Unit.each => material.GwpA123 * component.Amount,
                Unit.m => material.GwpA123 * component.Amount * element.Length,
                Unit.m2 => material.GwpA123 * component.Amount * element.Area,
                Unit.m3 => material.GwpA123 * component.Amount * element.Volume,
                _ => throw new Exception($"Unit not recognized: {unit}"),
            };
        }

        private static decimal CalculateCost(CalcElement element, BuildupComponent component, Unit unit)
        {
            var material = component.Material;
            return unit switch
            {
                Unit.each => material.Cost * component.Amount,
                Unit.m => material.Cost * component.Amount * element.Length,
                Unit.m2 => material.Cost * component.Amount * element.Area,
                Unit.m3 => material.Cost * component.Amount * element.Volume,
                _ => throw new Exception($"Unit not recognized: {unit}"),
            };
        }
    }
}
