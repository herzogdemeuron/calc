using Calc.Core.Calculation;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// make the flat snapshots for a branch (each element id, with element amount) or for a new assembly (each element type id)
    /// only merge the snapshots before sending to directus
    /// </summary>
    public class SnapshotMaker
    {
        /// <summary>
        /// generate the snapshots for a **dead end** branch
        /// </summary>
        public static void Snap(Branch branch) //move this to branch?
        {
            var rawSnapshots = new List<AssemblySnapshot>();
            foreach (var element in branch.Elements)
            
                foreach (var assembly in branch.Assemblies)
                {               
                    var s = MakeAssemblySnapshots(assembly, element, branch.ParentTree.Name);
                    rawSnapshots.AddRange(s);                    
                }            

            branch.AssemblySnapshots = MergeSnapshots(rawSnapshots);
        }

        /// <summary>
        /// generate the snapshots for an assembly, 
        /// the element type id should already be claimed in GetAssemblySnapshot
        /// </summary>
        public static void Snap(Assembly assembly) //move this to assembly?
        {
            var rawSnapshots = MakeAssemblySnapshots(assembly);
            assembly.AssemblySnapshot = MergeSnapshots(rawSnapshots);
        }

        /// <summary>
        /// make the raw snapshots (to be merged) for one assembly (of unit amount),
        /// for a branch, also claim the element and element group to the snapshot
        /// </summary>
        private static List<AssemblySnapshot> MakeAssemblySnapshots(Assembly assembly, CalcElement? element=null, string elementGroup=null)
        {
            var snapshots = new List<AssemblySnapshot>();
            foreach (var component in assembly.CalculationComponents)
            {
                var snapshot = CreateAssemblySnapshot(component, assembly);

                if (element != null && elementGroup != null)
                {
                    snapshot.ClaimElement(element.Value, elementGroup);
                }
                snapshots.Add(snapshot);
            }
            return snapshots;
        }

        /// <summary>
        /// create the assembly snapshot from a single calculation component in an assembly
        /// </summary>
        private static AssemblySnapshot CreateAssemblySnapshot(CalculationComponent caComponent, Assembly assembly)
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

            var aSnapshot = new AssemblySnapshot
            {
                AssemblyName = assembly.Name,
                AssemblyCode = assembly.Code,
                AssemblyGroup = assembly.Group.Name,
                AssemblyUnit = assembly.AssemblyUnit,
            };
            aSnapshot.AddMaterialSnapshot(materialSnapshot, caComponent.ElementTypeId);            

            return aSnapshot;
        }

        /// <summary>
        /// merge the assembly snapshots and their material snapshots
        /// </summary>
        public static List<AssemblySnapshot> MergeSnapshots(List<AssemblySnapshot> assemblySnapshots)
        {
            var result = MergeAssemblySnapshots(assemblySnapshots);
            foreach (var snapshot in result)
            {
                snapshot.MaterialSnapshots = MergeMaterialSnapshots(snapshot.MaterialSnapshots);
            }
            return result;
        }

        /// <summary>
        /// merge assembly snapshots according to the same element_group && assembly_code && element_type_id
        /// </summary>
        private static List<AssemblySnapshot> MergeAssemblySnapshots(List<AssemblySnapshot> assemblySnapshots)
        {
            var result = new List<AssemblySnapshot>();

            foreach (var aSnapshot in assemblySnapshots)
            {
                var existingSnapshot = result.Find(s =>
                    s.ElementTypeId == aSnapshot.ElementTypeId &&
                    s.AssemblyCode == aSnapshot.AssemblyCode &&
                    s.ElementGroup == aSnapshot.ElementGroup&&
                    s.AssemblyGroup == aSnapshot.AssemblyGroup
                    );

                if (existingSnapshot != null)
                {
                    existingSnapshot.ElementIds.AddRange(aSnapshot.ElementIds.Except(existingSnapshot.ElementIds));
                    existingSnapshot.MaterialSnapshots.AddRange(aSnapshot.MaterialSnapshots);
                }
                else
                {
                    result.Add(aSnapshot.Copy());
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
