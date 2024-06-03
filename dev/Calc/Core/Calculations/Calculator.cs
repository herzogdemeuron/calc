﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Calc.Core.Helpers;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;

namespace Calc.Core.Calculations
{
    public class Calculator
    {
        /// <summary>
        /// calculate gwp and ge for one branch
        /// store back the calculation
        /// this should only happen for dead end branches
        /// </summary>
        public static void Calculate(Branch branch)
        {
            if (branch == null) return;

            var results = new List<LayerResult>();
            var errorList = new List<ParameterError>();
            CalculateBranch(branch, results, errorList);

            branch.CalculationResults = results;
            branch.ParameterErrors = errorList;
        }

        private static void CalculateBranch(Branch branch, List<LayerResult> resultList, List<ParameterError> errorList)
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


        private static LayerResult GetResult(Branch branch, CalcElement element, Buildup buildup, CalculationComponent component, double amount)
        {
            var material = component.Material;
            var gwp = component.Gwp * amount;
            var ge = component.Ge * amount;
            var cost = component.Cost * amount;

            var calculationResult = new LayerResult
            {
                Forest = branch.ParentForest.Name,
                Tree = branch.ParentTree.Name,

                ElementId = element.Id,
                ElementType = element.TypeName,
                ElementUnit = buildup.BuildupUnit,
                ElementAmount = amount,

                BuildupName = buildup.Name,
                GroupName = buildup.Group?.Name,
                BuildupUnit = buildup.BuildupUnit,

                MaterialName = material.Name,
                MaterialUnit = component.Material.MaterialUnit,
                MaterialAmount = component.Amount??0,
                MaterialStandard = material.Standard.Name,
                MaterialSource = material.DataSource,
                MaterialSourceUuid = material.SourceUuid,
                MaterialFunction = component.Function.Name,
                MaterialGwp = material.Gwp??0,
                MaterialGe = material.Ge??0,

                Gwp = gwp??0,
                Ge = ge??0,
                //Cost = cost??0,
                //Color = branch.HslColor
            };
            return calculationResult;
        }

        /// <summary>
        /// get the layer result from a buildup component
        /// </summary>
        public static List<LayerResult> GetResult(double totalRatio, BuildupComponent buildupComponent, Unit buildupUnit, string buildupName, string buildupGroup)
        {
            var result = new List<LayerResult>();
            var layerComponents = buildupComponent.LayerComponents;
            if (layerComponents == null) return result;
            foreach ( var layerComponent in layerComponents)
            {
                var calculationComponents = layerComponent.CalculationComponents;
                if (calculationComponents == null) continue;
                foreach (var component in calculationComponents)
                {
                    var calculationResult = new LayerResult
                    {
                        ElementId = buildupComponent.TypeIdentifier.ToString(),
                        ElementType = $"{buildupComponent.Title} : {layerComponent.Title}",
                        ElementUnit = component.Material.MaterialUnit,
                        ElementAmount = (double)layerComponent.GetLayerAmount(totalRatio), // the layer amount is normalized to the total ratio

                        BuildupName = buildupName,
                        GroupName = buildupGroup,
                        BuildupUnit = buildupUnit,

                        MaterialName = component.Material.Name,
                        MaterialUnit = component.Material.MaterialUnit,
                        MaterialAmount = component.Amount ?? 0,
                        MaterialStandard = component.Material.Standard.Name,
                        MaterialSource = component.Material.DataSource,
                        MaterialSourceUuid = component.Material.SourceUuid,
                        MaterialFunction = component.Function.Name,
                        MaterialGwp = component.Material.Gwp ?? 0,
                        MaterialGe = component.Material.Ge ?? 0,

                        Gwp = component.Gwp ?? 0,
                        Ge = component.Ge ?? 0
                    };
                    result.Add(calculationResult);
                }
            }

            return result;

        }



       
    }
}
