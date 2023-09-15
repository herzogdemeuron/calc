﻿using System;
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
                            Forest = "test forest",
                            Tree = "test tree",

                            ElementId = element.Id,
                            ElementType = element.Type,
                            ElementUnit = buildup.Unit,
                            ElementQuantity = element.GetQuantityByUnit(buildup.Unit),

                            BuildupName = buildup.Name,
                            GroupName = buildup.Group.Name,
                            BuildupUnit = buildup.Unit,

                            MaterialName = material.Name,
                            MaterialCategory = material.Category,
                            MaterialGwp = material.KgCO2eA123,
                            MaterialUnit = material.Unit,
                            MaterialAmount = component.Amount,

                            Gwp = gwpA123,
                            Cost = cost,
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
            decimal quantity = element.GetQuantityByUnit(unit);
            return material.KgCO2eA123 * component.Amount * quantity;
        }

        private static decimal CalculateCost(CalcElement element, BuildupComponent component, Unit unit)
        {
            var material = component.Material;
            decimal quantity = element.GetQuantityByUnit(unit);
            return material.Cost * component.Amount * quantity;
        }
    }
}
