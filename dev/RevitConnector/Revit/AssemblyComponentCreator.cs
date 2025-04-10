﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.BasicParameters;
using Calc.RevitConnector.Config;
using Calc.RevitConnector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc.RevitConnector.Revit
{
    public class AssemblyComponentCreator 
    {
        private readonly Document Doc;
        private List<RevitBasicParamConfig> basicParamConfigs;
        public AssemblyComponentCreator(UIDocument uidoc)
        {
            Doc = uidoc.Document;
        }

        /// <summary>
        /// Creates a list of assembly components from a list of element ids.
        /// </summary>
        public List<AssemblyComponent> CreateAssemblyComponents(List<ElementId> ids, List<RevitBasicParamConfig> bParamConfigs)
        {
            basicParamConfigs = bParamConfigs;
            var result = new List<AssemblyComponent>();
            var elements = ids.Select(x => Doc.GetElement(x)).ToList();
            foreach (var element in elements)
            {
                var component = CreateAssemblyComponent(element);
                MergeAssemblyComponentToList(result, component);
            }
            return result;
        }

        /// <summary>
        /// Creates an assembly component from one element.
        /// </summary>
        private AssemblyComponent CreateAssemblyComponent(Element element)
        {
            var layersSet = CreateLayerComponents(element);
            var layers = layersSet.Item1;
            var isCompound = layersSet.Item2;
            var thickness = GetTypeThickness(element, isCompound);
            return new AssemblyComponent
            {
                Thickness = thickness,
                ElementIds = new List<int> { element.Id.IntegerValue },
                TypeIdentifier = element.GetTypeId().IntegerValue,
                Name = GetElementType(element)?.Name,
                LayerComponents = layers,
                IsCompoundElement = isCompound,
                BasicParameterSet = GetTotalAmounts(element) //Total amount of the assembly component is not sumed up from the layers, but directly from the element
            };
        }

        /// <summary>
        /// Merges an assembly component to a list of assembly components if they have the same type.
        /// </summary>
        private void MergeAssemblyComponentToList(List<AssemblyComponent> List, AssemblyComponent component)
        {
            var originComponent = List.FirstOrDefault(x => x.Equals(component));
            if (originComponent != null)
            {
                originComponent.BasicParameterSet.Add(component.BasicParameterSet);
                originComponent.ElementIds.AddRange(component.ElementIds);
                var isCompound = originComponent.IsCompoundElement;
                MergeLayerComponents(originComponent.LayerComponents, component.LayerComponents, isCompound);
            }
            else
            {
                List.Add(component);
            }            
        }

        /// <summary>
        /// Merges a layer component to a list of layer components with the same material.
        /// </summary>
        private void MergeLayerComponentWithMaterial(List<LayerComponent> List, LayerComponent component)
        {
            var sameComponent = List.FirstOrDefault(x => x.Equals(component));
            if (sameComponent != null)
            {
                sameComponent.BasicParameterSet.Add(component.BasicParameterSet);
            }
            else
            {
                List.Add(component);
            }
        }

        /// <summary>
        /// For compound elements:
        /// layers are defined by the compound structure layers in type property,
        /// merge two layer component lists if they are from the same compound structure type.
        /// For non compond elements:
        /// layers are defined by different materials,
        /// merge two layer component lists of layer components if they have the same material.
        /// </summary>
        private void MergeLayerComponents(List<LayerComponent> list1, List<LayerComponent> list2, bool isCompound)
        {
            if(isCompound)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    list1[i].BasicParameterSet.Add(list2[i].BasicParameterSet);
                }
            }
            else
            {
                foreach (var layerComponent in list2)
                {
                    MergeLayerComponentWithMaterial(list1, layerComponent);
                }

            }
        }

        /// <summary>
        /// Creates a list of layer components from an element.
        /// Firstly get the amounts for each material despite of the compound structure (materialAmounts),
        /// if there is a compound structure, re-order the material amount with the compund layers by the compound material thicknesses 
        /// and calculate the proportion of area and volume of the same material.
        /// </summary>
        /// <returns>bool: if the element is a compound structure.</returns>
        private (List<LayerComponent>,bool) CreateLayerComponents(Element elem)
        {
            bool isCompound = false;
            var result = new List<LayerComponent>();
            
            var compoundMaterials = GetCompoundMaterialThicknesses(elem);
            if (compoundMaterials != null)
            {
                var materialAmounts = GetMaterialAmounts(elem,true);
                isCompound = true;
                foreach (var (material, thickness, areaProp, volumeProp) in compoundMaterials)
                {
                    var materialAmount = materialAmounts.FirstOrDefault(x => x.Item1?.Id == material?.Id);
                    var paramSet = materialAmount.Item2;

                    // for compound, area and volume material amounts are trusted
                    var areaParam = paramSet.GetAmountParam(Unit.m2);
                    var volumeParam = paramSet.GetAmountParam(Unit.m3);

                    var newParamSet = new BasicParameterSet(
                        paramSet.GetAmountParam(Unit.piece), 
                        paramSet.GetAmountParam(Unit.m), 
                        areaParam.PerformOperation(Operation.Multiply, areaProp),
                        volumeParam.PerformOperation(Operation.Multiply, volumeProp)
                        );

                    var layerComponent = new LayerComponent()
                    {
                        TargetMaterialName = material?.Name,
                        BasicParameterSet = newParamSet,
                        Thickness = thickness
                    };
                    layerComponent.TypeIdentifier = elem.GetTypeId().IntegerValue.ToString();
                    result.Add(layerComponent);
                }
            }
            else
            {
                var materialAmounts = GetMaterialAmounts(elem, false);
                foreach (var (material, paramSet) in materialAmounts)
                {
                    var layerComponent = new LayerComponent()
                    {
                        TargetMaterialName = material?.Name,
                        BasicParameterSet = paramSet
                    };
                    layerComponent.TypeIdentifier = elem.GetTypeId().IntegerValue.ToString();
                    result.Add(layerComponent);
                }
            }
            return (result, isCompound);
        }

        /// <summary>
        /// Gets the material amounts of an element separated by different materials,
        /// by getting the material area and volume of the element.
        /// This step takes the area and volume of the material from revit, the area could be sometimes inaccurate.
        /// for compund elements and panels, the area is correct, in other cases, the area could be the whole area of the element.
        /// </summary>
        private List<(Material, BasicParameterSet)> GetMaterialAmounts(Element elem, bool isCompund)
        {
            var result = new List<(Material, BasicParameterSet)>();
            var totalAmountParamSet = GetTotalAmounts(elem);
            var materialIds = elem.GetMaterialIds(false);
            var countParamTotal = totalAmountParamSet.GetAmountParam(Unit.piece);
            var lengthParamTotal = totalAmountParamSet.GetAmountParam(Unit.m);
            var areaParamTotal = totalAmountParamSet.GetAmountParam(Unit.m2);
            var volumeParamTotal = totalAmountParamSet.GetAmountParam(Unit.m3);
            // firstly calculate valid material amounts
            foreach (var materialId in materialIds)
            {
                var material = Doc.GetElement(materialId) as Material;
                if (material == null) continue;
                var areaParam = ParameterHelper.CreateMaterialAmountParameter(elem, materialId, Unit.m2);
                var volumeParam = ParameterHelper.CreateMaterialAmountParameter(elem, materialId, Unit.m3);
                var materialAmount = new BasicParameterSet(countParamTotal, lengthParamTotal, areaParam, volumeParam);
                result.Add((material, materialAmount));
            }
            // calculate the no material amounts by subtracting the material amounts from the total amounts
            foreach (var materialAmount in result.Select(x => x.Item2))
            {
                var volumeParam = materialAmount.GetAmountParam(Unit.m3);
                volumeParamTotal = volumeParamTotal.PerformOperation(Operation.Subtract, volumeParam);
            }
            // show no material if volume has value
            // the no material amounts are all missing params, they shall not be used for calculation
            if (!volumeParamTotal.HasError && volumeParamTotal.Amount > 0)
            {
                result.Add( (null, BasicParameterSet.ErrorParamSet()) );
            }
            // the material area values are correct if it is a compound structure
            if(isCompund) return result;
            // otherwise, if there is only one material, take the element area as the material area
            // if more than one material, make the area param as error
            if ( (result.Count == 1) && (result[0].Item1 != null))
            {
                result[0].Item2.Set(areaParamTotal);
            }
            else
            {
                var areaParam = BasicParameter.ErrorParam(Unit.m2);
                result.ForEach(x => x.Item2.Set(areaParam));
            }
            return result;
        }

        /// <summary>
        /// Gets the total amounts of an element, including count, length, area and volume using the basic parameter config.
        /// </summary>
        private BasicParameterSet GetTotalAmounts(Element elem)
        {
            var paramConfig = GetParamConfig(elem);
            var lengthParam = ParameterHelper.CreateBasicUnitParameter(elem, paramConfig.LengthName, Unit.m);
            var areaParam = ParameterHelper.CreateBasicUnitParameter(elem, paramConfig.AreaName, Unit.m2);
            var volumeParam = ParameterHelper.CreateBasicUnitParameter(elem, paramConfig.VolumeName, Unit.m3);
            var countParam = ParameterHelper.CreateBasicUnitParameter(elem, paramConfig.CountName, Unit.piece);
            return new BasicParameterSet(countParam, lengthParam, areaParam, volumeParam);
        }

        /// <summary>
        /// Gets the parameter config for the category of the element.
        /// </summary>
        private RevitBasicParamConfig GetParamConfig(Element elem)
        {
            foreach (var config in basicParamConfigs)
            {
                if (elem.Category.Id.IntegerValue == (int)config.category)
                {
                    return config;
                }
            }
            return new RevitBasicParamConfig();
        }

        /// <summary>
        /// Gets the compound material thicknesses (mm) of the element with the right order.
        /// </summary>
        /// <returns>The material, its thickness and the proportion of area and volume of the same material</returns>
        private List<(Material,double,double,double)> GetCompoundMaterialThicknesses(Element elem, int round=3)
        {
            var type = GetElementType(elem);
            if (type == null) return null;
            var result = new List<(Material, double, double, double)>();
            var materials = new List<(Material, double)>();
            if (type is HostObjAttributes hostObjAttributes)
            {
                var compoundStructure = hostObjAttributes.GetCompoundStructure();
                if (compoundStructure == null) return null;
                var layers = compoundStructure.GetLayers();
                foreach (var layer in layers)
                {
                    var materialId = layer.MaterialId;
                    var material = Doc.GetElement(materialId) as Material;
                    var width = ParameterHelper.ToMetricValue(layer.Width, Unit.m) * 1000; // convert to mm
                    width = Math.Round(width, round);
                    materials.Add((material, width));
                }
                foreach (var material in materials)
                {
                    var props = GetMaterialRatio(materials, material);

                    result.Add((material.Item1, material.Item2, props.Item1, props.Item2));
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Gets the proportion of the area and volume of the same material.
        /// For area, devided by same material layer count if the material is valid, otherwise devided by 1.
        /// For volume, devided by the thickness of the same material.
        /// </summary>
        private (double,double) GetMaterialRatio(List<(Material, double)> materials, (Material, double) singleMaterial)
        {
            // get the thickness of the same material
            var sameMaterial = materials.Where(x => x.Item1?.Id == singleMaterial.Item1?.Id);
            var sameMaterialCount = sameMaterial.Count();
            if(sameMaterialCount == 0) return (0,0);

            if(singleMaterial.Item1 == null) sameMaterialCount = 1;

            var areaProportion = 1 / (double)sameMaterialCount;

            var wholeMaterialThickness = sameMaterial.Sum(x => x.Item2);
            var volumeProportion = wholeMaterialThickness == 0? 0: singleMaterial.Item2 / wholeMaterialThickness;

            return (areaProportion, volumeProportion);
        }

        /// <summary>
        /// Gets the thickness of the element type in mm.
        /// </summary>
        private double? GetTypeThickness(Element elem, bool isCompound)
        {
            if (isCompound)
            {
                var type = GetElementType(elem);
                if (type == null) return null;
                var thickness = type is FloorType || type is RoofType || type is CeilingType ?
                    type.LookupParameter("Default Thickness")?.AsDouble() ?? 0 :
                    type.LookupParameter("Width")?.AsDouble() ?? 0;                
                if (thickness == 0) return null;
                return ParameterHelper.ToMetricValue(thickness, Unit.m) * 1000; // convert to mm
            }
            return null;
        }

        private Element GetElementType(Element elem)
        {
            ElementId typeId = elem.GetTypeId();
            return typeId == ElementId.InvalidElementId ? null: Doc.GetElement(typeId);
        }
    }
}
