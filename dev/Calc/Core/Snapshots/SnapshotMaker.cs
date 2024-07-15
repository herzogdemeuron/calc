using Calc.Core.Calculation;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
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

            var snapshots = new List<BuildupSnapshot>();
            foreach (var element in branch.Elements)
            
                foreach (var buildup in branch.Buildups)
                {               
                    var s = MakeBuildupSnapshot(buildup, element);
                    snapshots.AddRange(s);                    
                }            

            branch.BuildupSnapshots = MergeSnapshots(snapshots);
        }


        /// <summary>
        /// generate the snapshots for a buildup, 
        /// the element type id should already be claimed in GetBuildupSnapshot
        /// </summary>
        public static void Snap(Buildup buildup)
        {
            var snapshots = MakeBuildupSnapshot(buildup);
            buildup.BuildupSnapshot = MergeSnapshots(snapshots);
        }


        /// <summary>
        /// make the snapshots for a buildup (of unit amount), (for a branch) claim the element to the snapshot
        /// </summary>
        private static List<BuildupSnapshot> MakeBuildupSnapshot(Buildup buildup, CalcElement? element=null)
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
        /// get the buildup snapshot from a single calculation component in a buildup
        /// </summary>
        private static BuildupSnapshot GetBuildupSnapshot(CalculationComponent caComponent, Buildup buildup)
        {
            var calculationComponents = buildup.CalculationComponents;

            var material = caComponent.Material;
            var materialSnapshot = new MaterialSnapshot
            {
                MaterialFunction = caComponent.Function.Name,
                MaterialSourceUuid = material.SourceUuid,
                MaterialSource = material.DataSource,
                MaterialName = material.Name,
                MaterialUnit = material.MaterialUnit,
                MaterialAmount = caComponent.Amount,
                MaterialGwp = material.Gwp,
                MaterialGe = material.Ge,
                CalculatedGwp = caComponent.Gwp,
                CalculatedGe = caComponent.Ge
            };

            var bSnapshot = new BuildupSnapshot
            {
                BuildupName = buildup.Name,
                BuildupCode = buildup.Code,
                BuildupGroup = buildup.Group.Name,
                BuildupUnit = buildup.BuildupUnit,
                MaterialSnapshots = new List<MaterialSnapshot> { materialSnapshot }
            };

            if (caComponent.ElementTypeId != null)
            {
                // the element type id should be claimed
                // if the calculation component was generated from layer component
                bSnapshot.ClaimElementTypeId(caComponent.ElementTypeId);
            }

            return bSnapshot;
        }

        /// <summary>
        /// merge the buildup snapshots and their material snapshots
        /// </summary>
        private static List<BuildupSnapshot> MergeSnapshots(List<BuildupSnapshot> buildupSnapshots)
        {
            var result = MergeBuildupSnapshots(buildupSnapshots);
            foreach (var snapshot in result)
            {
                snapshot.MaterialSnapshots = MergeMaterialSnapshots(snapshot.MaterialSnapshots);
            }
            return result;
        }

        /// <summary>
        /// merge buildup snapshots according to the same element_group && buildup_code && element_type_id
        /// </summary>
        private static List<BuildupSnapshot> MergeBuildupSnapshots(List<BuildupSnapshot> buildupSnapshots)
        {
            var result = new List<BuildupSnapshot>();

            foreach (var bSnapshot in buildupSnapshots)
            {
                var existingSnapshot = result.Find(s =>
                    s.ElementTypeId == bSnapshot.ElementTypeId &&
                    s.BuildupCode == bSnapshot.BuildupCode &&
                    s.ElementGroup == bSnapshot.ElementGroup&&
                    s.BuildupGroup == bSnapshot.BuildupGroup
                    );

                if (existingSnapshot != null)
                {
                    existingSnapshot.ElementIds.AddRange(bSnapshot.ElementIds);

                    existingSnapshot.MaterialSnapshots.AddRange(bSnapshot.MaterialSnapshots);
                }
                else
                {
                    result.Add(bSnapshot.Copy());
                }
            }

            return result;
        }

        /// <summary>
        /// merge the material snapshots with the same material_function && material_source_uuid && material_source
        /// </summary>
        private static List<MaterialSnapshot> MergeMaterialSnapshots(List<MaterialSnapshot> materialSnapshots)
        {
            var result = new List<MaterialSnapshot>();

            foreach (var mSnapshot in materialSnapshots)
            {
                var existingMaterial = result.Find(m =>
                    m.MaterialSourceUuid == mSnapshot.MaterialSourceUuid &&
                    m.MaterialSource == mSnapshot.MaterialSource &&
                    m.MaterialFunction == mSnapshot.MaterialFunction);

                if (existingMaterial != null)
                {
                    existingMaterial.MaterialAmount += mSnapshot.MaterialAmount;
                    existingMaterial.CalculatedGwp += mSnapshot.CalculatedGwp;
                    existingMaterial.CalculatedGe += mSnapshot.CalculatedGe;
                }
                else
                {
                    result.Add(mSnapshot.Copy());
                }
            }
            return result;
        }

        /// <summary>
        /// flatten the buildup snapshots to a list of merged material snapshots
        /// </summary>
        public static List<MaterialSnapshot> FlattenBuilupSnapshots(List<BuildupSnapshot> snapshots)
        {
            var result = new List<MaterialSnapshot>();
            foreach (var snapshot in snapshots)
            {
                result.AddRange(snapshot.MaterialSnapshots);
            }
            return MergeMaterialSnapshots(result);
        }
    }
}
