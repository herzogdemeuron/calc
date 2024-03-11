using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.RevitConnector.Config;
using Calc.RevitConnector.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Calc.RevitConnector.Revit
{
    public class BuildupComponentCreator : IBuildupComponentCreator

    {
        /// <summary>
        /// Create a list of calc elements from the current document
        /// it only takes the relevant parameters from the parameter name list to speed up the process
        /// </summary>
        
        private readonly Document Doc;
        private readonly UIDocument Uidoc;
        private const string noMaterialName = "No Material";
        private readonly List<RevitBasicParamConfig> basicParamConfigs =
            new List<RevitBasicParamConfig>
            {
            new RevitBasicParamConfig(BuiltInCategory.OST_Doors, AreaName: ".Area"),
            new RevitBasicParamConfig(BuiltInCategory.OST_Windows, AreaName: ".Area")
            };



        public BuildupComponentCreator(UIDocument uidoc)
        {
            Doc = uidoc.Document;
            Uidoc = uidoc;
        }


        /// <summary>
        /// prompt the user to select elements from revit, and return a list of buildup components.
        /// </summary>
        public List<BuildupComponent> CreateBuildupComponentsFromSelection()
        {
            var ids = SelectionHelper.SelectElements(Uidoc);
            return CreateBuildupComponents(ids);
        }

        /// <summary>
        /// create a list of buildup components from a list of element ids
        /// </summary>
        private List<BuildupComponent> CreateBuildupComponents(List<ElementId> ids)
        {
            var result = new List<BuildupComponent>();
            var elements = ids.Select(x => Doc.GetElement(x)).ToList();
            foreach (var element in elements)
            {
                var component = CreateBuildupComponent(element);
                MergeBuildupComponentToList(result, component);
            }
            return result;
        }


        /// <summary>
        /// create a buildup component from one element
        /// </summary>
        private BuildupComponent CreateBuildupComponent(Element element)
        {
            var layers = CreateLayerComponents(element);
            return new BuildupComponent
            {
                ElementIds = new List<int> { element.Id.IntegerValue },
                TypeIdentifier = element.GetTypeId().IntegerValue,
                Name = GetElementType(element)?.Name,
                LayersComponent = layers,
                //BasicParameterSet = GetTotalAmounts(element)
            };

        }

        /// <summary>
        /// merge a buildup component to a list of buildup components if they have the same type
        /// </summary>
        private void MergeBuildupComponentToList(List<BuildupComponent> List, BuildupComponent component)
        {
            var sameComponent = List.FirstOrDefault(x => x.CheckType(component));
            if (sameComponent != null)
            {
                //sameComponent.BasicParameterSet.Add(component.BasicParameterSet);
                sameComponent.ElementIds.AddRange(component.ElementIds);
                foreach (var layerComponent in component.LayersComponent)
                {
                    MergeLayerComponentToList(sameComponent.LayersComponent, layerComponent);
                }
            }
            else
            {
                List.Add(component);
            }            
        }

        /// <summary>
        /// merge a layer component to a list of layer components if they have the same material
        /// </summary>
        private void MergeLayerComponentToList(List<LayerComponent> List, LayerComponent component)
        {
            var sameComponent = List.FirstOrDefault(x => x.TargetMaterialName == component.TargetMaterialName);
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
        /// create a list of layer components from an element.
        /// Firstly get the amounts for each material despite of the compound structure (materialAmounts),
        /// if there is a compound structure, re-order the material amount with the compund layers by the compound material thicknesses and calculate the proportion of area and volume of the same material.
        /// </summary>
        private List<LayerComponent> CreateLayerComponents(Element elem)
        {
            var result = new List<LayerComponent>();
            var materialAmounts = GetMaterialAmounts(elem);
            var materials = GetCompoundMaterialThicknesses(elem);
            if (materials != null)
            {
                foreach (var (material, thickness, areaProp, volumeProp) in materials)
                {
                    var materialAmount = materialAmounts.FirstOrDefault(x => x.Item1?.Id == material?.Id);
                    var paramSet = materialAmount.Item2;
                    var areaParam = paramSet.GetAmountParam(Unit.m2);
                    var volumeParam = paramSet.GetAmountParam(Unit.m3);

                    var newParamSet = new BasicParameterSet(
                        paramSet.GetAmountParam(Unit.piece), 
                        paramSet.GetAmountParam(Unit.m), 
                        areaParam.PerformOperation(Operation.Multiply, areaProp),
                        volumeParam.PerformOperation(Operation.Multiply, volumeProp)
                        );

                    var layerComponent = new LayerComponent(material?.Name??noMaterialName, newParamSet, thickness);
                    result.Add(layerComponent);
                }
            }
            else
            {
                foreach (var (material, paramSet) in materialAmounts)
                {
                    var layerComponent = new LayerComponent(material?.Name ?? noMaterialName, paramSet);
                    result.Add(layerComponent);
                }
            }
            return result;
        }

        /// <summary>
        /// get the material amounts of an element separated by different materials by getting the material area and volume of the element.
        /// </summary>
        private List<(Material, BasicParameterSet)> GetMaterialAmounts(Element elem)
        {
            var totalAmountParamSet = GetTotalAmounts(elem);
            var materialIds = elem.GetMaterialIds(false);
            var countParamTotal = totalAmountParamSet.GetAmountParam(Unit.piece);
            var lengthParamTotal = totalAmountParamSet.GetAmountParam(Unit.m);
            var result = new List<(Material, BasicParameterSet)>();
            foreach (var materialId in materialIds)
            {
                var material = Doc.GetElement(materialId) as Material;
                if(material == null) continue;

                var materialArea = elem.GetMaterialArea(materialId, false);
                var areaParam = new BasicParameter()
                {
                    Name = "From Material",
                    Unit = Unit.m2,
                    Amount = materialArea
                };

                var materialVolume = elem.GetMaterialVolume(materialId);
                var volumeParam = new BasicParameter()
                {
                    Name = "From Material",
                    Unit = Unit.m3,
                    Amount = materialVolume
                };

                var materialAmount = new BasicParameterSet(countParamTotal, lengthParamTotal, areaParam, volumeParam);

                result.Add((material, materialAmount));
            }

            // calculate the no material amounts by subtracting the material amounts from the total amounts
            var areaParamTotal = totalAmountParamSet.GetAmountParam(Unit.m2);
            var volumeParamTotal = totalAmountParamSet.GetAmountParam(Unit.m3);
            foreach (var materialAmount in result.Select(x => x.Item2))
            {
                var areaParam = materialAmount.GetAmountParam(Unit.m2);
                var volumeParam = materialAmount.GetAmountParam(Unit.m3);
                areaParamTotal = areaParamTotal.PerformOperation(Operation.Subtract, areaParam);
                volumeParamTotal = volumeParamTotal.PerformOperation(Operation.Subtract, volumeParam);
            }

            // show no material if either area or volume has value
            if (!(areaParamTotal.HasError && volumeParamTotal.HasError))
            {
                var noMaterialAmount = new BasicParameterSet(countParamTotal, lengthParamTotal, areaParamTotal, volumeParamTotal);
                result.Add((null, noMaterialAmount));
            }
 
            return result;
        }


        /// <summary>
        /// get the total amounts of an element, including count, length, area and volume using the basic parameter config.
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
        /// get the parameter config for the category of the element
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
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
        /// get the compound material thicknesses of the element with the right order,
        /// returning the material, its thickness and the proportion of area and volume of the same material
        /// </summary>
        private List<(Material,double,double,double)> GetCompoundMaterialThicknesses(Element elem)
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
                    materials.Add((material, layer.Width));
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
        /// get the proportion of the area and volume of the same material
        /// for material, devided by same material layer count
        /// for volume, devided by the thickness of the same material
        /// </summary>
        private (double,double) GetMaterialRatio(List<(Material, double)> materials, (Material, double) singleMaterial)
        {
            // get the thickness of the same material
            var sameMaterial = materials.Where(x => x.Item1?.Id == singleMaterial.Item1?.Id);
            var sameMaterialCount = sameMaterial.Count();
            if(sameMaterialCount == 0) return (0,0);

            var areaProportion = 1 / sameMaterialCount;

            var wholeMaterialThickness = sameMaterial.Sum(x => x.Item2);
            var volumeProportion = wholeMaterialThickness == 0? 0: singleMaterial.Item2 / wholeMaterialThickness;

            return (areaProportion, volumeProportion);
        }

        private Element GetElementType(Element elem)
        {
            ElementId typeId = elem.GetTypeId();
            return typeId == ElementId.InvalidElementId ? null: Doc.GetElement(typeId);

        }
    }
}
