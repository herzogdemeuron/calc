using Calc.Core.Calculation;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;
using System.Collections.Generic;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// make the flat snapshots for a branch (each element) or for a new buildup (each element type)
    /// only merge the snapshots before sending to directus
    /// </summary>
    public class SnapshotMaker
    {
        /// <summary>
        /// generate the snapshots for a **dead end** branch, 
        /// </summary>
        public static void Snap(Branch branch) //move this to branch?
        {

            var resultList = new List<BuildupSnapshot>();
            foreach (var element in branch.Elements)
            
                foreach (var buildup in branch.Buildups)
                {               
                    var snapshots = GetBuildupSnapshot(buildup, element);
                    resultList.AddRange(snapshots);                    
                }
            

            branch.BuildupSnapshots = resultList;
        }


        /// <summary>
        /// generate the snapshots for a buildup, claim the element type id
        /// </summary>
        /// <param name="buildup"></param>
        public static void Snap(Buildup buildup)
        {
            var snapshots = GetBuildupSnapshot(buildup);
            buildup.BuildupSnapshot = snapshots;
        }


        /// <summary>
        /// make the snapshots for a branch, claim the element to the snapshots if given
        /// </summary>
        public static List<BuildupSnapshot> GetBuildupSnapshot(Buildup buildup, CalcElement? element=null)
        {
            var snapshots = new List<BuildupSnapshot>();
            foreach (var component in buildup.CalculationComponents)
            {
                var snapshot = GetBuildupSnapshot(component, buildup);
                if (element != null)
                {
                    snapshot.ClaimElement(element.Value);
                }
                snapshots.Add(snapshot);
            }
            return snapshots;
        }


        /// <summary>
        /// get the buildup snapshot from a single calculation component
        /// </summary>
        private static BuildupSnapshot GetBuildupSnapshot(CalculationComponent component, Buildup buildup)
        {
            var calculationComponents = buildup.CalculationComponents;

            var material = component.Material;
            var materialSnapshot = new MaterialSnapshot
            {
                MaterialFunction = component.Function.Name,
                MaterialSourceUuid = material.SourceUuid,
                MaterialSource = material.DataSource,
                MaterialName = material.Name,
                MaterialUnit = material.MaterialUnit,
                MaterialAmount = component.Amount,
                MaterialGwp = material.Gwp,
                MaterialGe = material.Ge,
                Gwp = component.Gwp,
                Ge = component.Ge
            };

            var snapshot = new BuildupSnapshot
            {
                BuildupName = buildup.Name,
                BuildupCode = buildup.Code,
                BuildupGroup = buildup.Group.Name,
                BuildupUnit = buildup.BuildupUnit,
                BuildupGwp = buildup.BuildupGwp,
                BuildupGe = buildup.BuildupGe,
                MaterialSnapshots = new List<MaterialSnapshot> { materialSnapshot }
            };

            if (component.ElementTypeId != null)
            {
                // the element type id should be claimed
                // if the calculation component was generated from layer component
                snapshot.ClaimElementTypeId(component.ElementTypeId);                                                                      
            }

            return snapshot;
        }


















        // deprecated
        public static List<LayerResult> GetResult(double totalRatio, BuildupComponent buildupComponent, Unit buildupUnit, string buildupName, string buildupCode, string buildupGroup)
        {
            var result = new List<LayerResult>();
            var layerComponents = buildupComponent.LayerComponents;
            if (layerComponents == null) return result;
            foreach (var layerComponent in layerComponents)
            {
                var calculationComponents = layerComponent.CalculationComponents;
                if (calculationComponents == null) continue;
                foreach (var component in calculationComponents)
                {
                    var calculationResult = new LayerResult
                    {
                        ElementTypeId = buildupComponent.TypeIdentifier.ToString(),
                        ElementType = $"{buildupComponent.Title} : {layerComponent.Title}",
                        ElementUnit = component.Material.MaterialUnit,
                        ElementAmount = (double)layerComponent.GetLayerAmount(totalRatio), // the layer amount is normalized to the total ratio

                        BuildupName = buildupName,
                        BuildupCode = buildupCode,
                        GroupName = buildupGroup,
                        BuildupUnit = buildupUnit,

                        MaterialName = component.Material.Name,
                        MaterialUnit = component.Material.MaterialUnit,
                        MaterialAmount = component.Amount,
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
