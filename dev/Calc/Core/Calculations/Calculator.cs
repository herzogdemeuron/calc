using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    if (buildup?.Components == null) continue;
                    foreach (var component in buildup.Components)
                    {
                        if (component == null) continue;

                        BasicUnitParameter param = element.GetBasicUnitParameter(buildup.Unit);

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

                        var quantity = param.Value;
                        var calculationResult = GetResult(branch, element, buildup, component, quantity.Value);

                        resultList.Add(calculationResult);
                    }
                }
            }
        }

        private static Result GetResult(Branch branch, CalcElement element, Buildup buildup, BuildupComponent component, decimal quantity)
        {
            var material = component.Material;
            var gwpA123 = material.GWP * component.Amount * quantity;
            var geA123 = material.GE * component.Amount * quantity;
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
                MaterialGwp = material.GWP,
                MaterialUnit = material.Unit,
                MaterialAmount = component.Amount,

                Gwp = gwpA123,
                Ge = geA123,
                Cost = cost,
                Color = branch.HslColor
            };
            return calculationResult;
        }

       
    }
}
