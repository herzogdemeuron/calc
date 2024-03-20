using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calc.Core.Helpers;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;

namespace Calc.Core.Calculations
{
    public class Calculator
    {
        /// <summary>
        /// calculate gwp and cost for one branch
        /// store back the calculation to it
        /// this should only happen for dead end branches
        /// </summary>
        public static void Calculate(Branch branch)
        {
            if (branch == null) return;

            var results = new List<Result>();
            var errorList = new List<ParameterError>();
            CalculateBranch(branch, results, errorList);

            branch.CalculationResults = results;
            branch.ParameterErrors = errorList;
        }

        private static void CalculateBranch(Branch branch, List<Result> resultList, List<ParameterError> errorList)
        {
            var buildups = branch.Buildups;
            if (buildups == null) return;

            foreach (var element in branch.Elements)
            {
                foreach (var buildup in buildups)
                {
                    foreach (var component in buildup.CalculationComponents)
                    {
                        if (component == null) continue;

                        BasicParameter param = element.GetBasicUnitParameter(buildup.BuildupUnit);

                        if (param.ErrorType != null)
                        {
                            ParameterErrorHelper.AddToErrorList
                                (
                                errorList, 
                                new ParameterError
                                    {
                                        ParameterName = param.Name,
                                        Unit = param.Unit,
                                        ErrorType = param.ErrorType,
                                        ElementIds = new List<string> { element.Id }
                                    }
                                );
                            continue;
                        }

                        var amount = param.Amount;
                        var calculationResult = GetResult(branch, element, buildup, component, amount.Value);

                        resultList.Add(calculationResult);
                    }
                }
            }
        }

        // needs to be validaed. amount instead of quantity?
        private static Result GetResult(Branch branch, CalcElement element, Buildup buildup, CalculationComponent component, double quantity)
        {
            var material = component.Material;
            var gwpA123 = component.Gwp * quantity;
            var geA123 = component.Ge * quantity;
            var cost = component.Cost * quantity;

            var calculationResult = new Result
            {
                Forest = branch.ParentForest.Name,
                Tree = branch.ParentTree.Name,

                ElementId = element.Id,
                ElementType = element.TypeName,
                ElementUnit = buildup.BuildupUnit,
                ElementQuantity = quantity,

                BuildupName = buildup.Name,
                GroupName = buildup.Group?.Name,
                BuildupUnit = buildup.BuildupUnit,

                MaterialName = material.Name,
                MaterialSource = material.Standard.Name,
                //MaterialSourceCode = material.SourceCode,
                MaterialCategory = material.Category,
                MaterialGwp = material.Gwp??0,
                MaterialUnit = component.Material.MaterialUnit,
                MaterialAmount = component.Amount??0,

                Gwp = gwpA123??0,
                Ge = geA123??0,
                Cost = cost??0,
                Color = branch.HslColor
            };
            return calculationResult;
        }

       
    }
}
