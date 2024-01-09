using System;
using System.Collections.Generic;
using System.Linq;
using Calc.Core.Helpers;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;

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
                //results.AddRange(Calculate(branch));
            }
            return results;
        }

        /// <summary>
        /// calculate gwp and cost for one branch
        /// store back the calculation to it
        /// this should only happen for dead end branches
        /// </summary>
        public static void Calculate(Branch branch)
        {
            if (branch == null) return;

            var results = new List<Result>();
            var zeroQuantityElements = new Dictionary<string, List<string>>();
            var nullQuantityElements = new Dictionary<string, List<string>>();

         
            CalculateBranch(branch, results, zeroQuantityElements, nullQuantityElements);
            

            branch.CalculationResults = results;
            branch.CalculationZeroElements = zeroQuantityElements;
            branch.CalculationNullElements = nullQuantityElements;
        }

        private static void CalculateBranch(Branch branch, List<Result> resultList, Dictionary<string, List<string>> zeroList, Dictionary<string, List<string>> nullList)
        {
            var buildups = branch.Buildups;
            if (buildups == null) return;

            foreach (var element in branch.Elements)
            {
                foreach (var buildup in buildups)
                {
                    if (buildup?.Components == null) continue;
                    foreach (var component in buildup.Components)
                    {
                        if (component == null) continue;

                        BasicUnitParameter param = element.GetBasicUnitParameter(buildup.Unit);

                        var quantity = param.Value;
                        var paramName = param.Name;

                        if (quantity == 0)
                        {
                            CollectionHelper.AddToCountDict(zeroList, paramName, element.Id);
                            continue;
                        }
                        if (quantity == null)
                        {
                            CollectionHelper.AddToCountDict(nullList, paramName, element.Id);
                            continue;
                        }

                        var calculationResult = GetResult(branch, element, buildup, component, quantity.Value);

                        resultList.Add(calculationResult);
                    }
                }
            }
        }

        private static Result GetResult(Branch branch, CalcElement element, Buildup buildup, BuildupComponent component, decimal quantity)
        {
            var material = component.Material;
            var gwpA123 = material.KgCO2eA123 * component.Amount * quantity;
            var cost = material.Cost * component.Amount * quantity;

            var calculationResult = new Result
            {
                Forest = branch.ParentForest.Name,
                Tree = branch.ParentTree.Name,

                ElementId = element.Id,
                ElementType = element.TypeName,
                ElementUnit = buildup.Unit,
                ElementQuantity = quantity,

                BuildupName = buildup.Name,
                GroupName = buildup.Group?.Name,
                BuildupUnit = buildup.Unit,

                MaterialName = material.Name,
                MaterialSource = material.Source,
                MaterialSourceCode = material.SourceCode,
                MaterialCategory = material.Category,
                MaterialGwp = material.KgCO2eA123,
                MaterialUnit = material.Unit,
                MaterialAmount = component.Amount,

                Gwp = gwpA123,
                Cost = cost,
                Color = branch.HslColor
            };
            return calculationResult;
        }

       
    }
}
