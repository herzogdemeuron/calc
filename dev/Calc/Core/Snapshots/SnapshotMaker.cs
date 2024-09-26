using Calc.Core.Calculation;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// make the flat snapshots for a branch (each element) or for a new assembly (each element type)
    /// only merge the snapshots before sending to directus
    /// </summary>
    public class SnapshotMaker
    {
        /// <summary>
        /// generate the snapshots for a **dead end** branch, 
        /// </summary>
        public static void Snap(Branch branch) //move this to branch?
        {

            var snapshots = new List<AssemblySnapshot>();
            foreach (var element in branch.Elements)
            
                foreach (var assembly in branch.Assemblies)
                {               
                    var s = MakeBuildupSnapshot(assembly, element, branch.ParentTree.Name);
                    snapshots.AddRange(s);                    
                }            

            branch.BuildupSnapshots = MergeSnapshots(snapshots);
        }


        /// <summary>
        /// generate the snapshots for a assembly, 
        /// the element type id should already be claimed in GetBuildupSnapshot
        /// </summary>
        public static void Snap(Assembly assembly)
        {
            var snapshots = MakeBuildupSnapshot(assembly);
            assembly.BuildupSnapshot = MergeSnapshots(snapshots);
        }


        /// <summary>
        /// make the snapshots for a assembly (of unit amount), (for a branch) claim the element to the snapshot
        /// </summary>
        private static List<AssemblySnapshot> MakeBuildupSnapshot(Assembly assembly, CalcElement? element=null, string elementGroup=null)
        {
            var snapshots = new List<AssemblySnapshot>();
            foreach (var component in assembly.CalculationComponents)
            {
                var snapshot = GetBuildupSnapshot(component, assembly);
                if (element != null && elementGroup != null)
                {
                    snapshot.ClaimElement(element.Value, elementGroup);
                }
                snapshots.Add(snapshot);
            }
            return snapshots;
        }

        /// <summary>
        /// get the assembly snapshot from a single calculation component in a assembly
        /// </summary>
        private static AssemblySnapshot GetBuildupSnapshot(CalculationComponent caComponent, Assembly assembly)
        {
            var calculationComponents = assembly.CalculationComponents;

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

            var bSnapshot = new AssemblySnapshot
            {
                BuildupName = assembly.Name,
                BuildupCode = assembly.Code,
                BuildupGroup = assembly.Group.Name,
                BuildupUnit = assembly.BuildupUnit,
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
        /// merge the assembly snapshots and their material snapshots
        /// </summary>
        public static List<AssemblySnapshot> MergeSnapshots(List<AssemblySnapshot> assemblySnapshots)
        {
            var result = MergeBuildupSnapshots(assemblySnapshots);
            foreach (var snapshot in result)
            {
                snapshot.MaterialSnapshots = MergeMaterialSnapshots(snapshot.MaterialSnapshots);
            }
            return result;
        }

        /// <summary>
        /// merge assembly snapshots according to the same element_group && assembly_code && element_type_id
        /// </summary>
        private static List<AssemblySnapshot> MergeBuildupSnapshots(List<AssemblySnapshot> assemblySnapshots)
        {
            var result = new List<AssemblySnapshot>();

            foreach (var bSnapshot in assemblySnapshots)
            {
                var existingSnapshot = result.Find(s =>
                    s.ElementTypeId == bSnapshot.ElementTypeId &&
                    s.BuildupCode == bSnapshot.BuildupCode &&
                    s.ElementGroup == bSnapshot.ElementGroup&&
                    s.BuildupGroup == bSnapshot.BuildupGroup
                    );

                if (existingSnapshot != null)
                {
                    existingSnapshot.ElementIds.AddRange(bSnapshot.ElementIds.Except(existingSnapshot.ElementIds));
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
        /// flatten the assembly snapshots to a list of merged material snapshots
        /// </summary>
        public static List<MaterialSnapshot> FlattenBuilupSnapshots(List<AssemblySnapshot> snapshots)
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
