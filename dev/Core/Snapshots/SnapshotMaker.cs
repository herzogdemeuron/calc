using Calc.Core.Calculation;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Snapshots
{
    /// <summary>
    /// Makes the flat snapshots for a branch (each element id, with element amount) or for a new assembly (each element type id)
    /// only merge the snapshots before sending to directus
    /// </summary>
    public class SnapshotMaker
    {
        /// <summary>
        /// Generates the snapshots for a **dead end** branch
        /// </summary>
        public static void Snap(Branch branch) // todo: move this to branch?
        {
            var rawSnapshots = new List<AssemblySnapshot>();
            foreach (var element in branch.Elements)
            
                foreach (var assembly in branch.Assemblies)
                {
                    var s = CreateAssemblySnapshots(assembly, element, branch.Query.Name);
                    rawSnapshots.AddRange(s);
                }

            branch.AssemblySnapshots = MergeAssemblySnapshots(rawSnapshots);
        }

        /// <summary>
        /// Generates the snapshots for an assembly, 
        /// the element type id should already be claimed in GetAssemblySnapshot.
        /// </summary>
        public static void Snap(Assembly assembly) // todo: move this to assembly?
        {
            var rawSnapshots = CreateAssemblySnapshots(assembly);
            assembly.AssemblySnapshot = MergeAssemblySnapshots(rawSnapshots).FirstOrDefault();
        }

        /// <summary>
        /// Make the raw snapshots (raw means to be merged) for one assembly (of unit amount),
        /// for a branch, also claim the element and element group to the snapshot.
        /// </summary>
        private static List<AssemblySnapshot> CreateAssemblySnapshots(Assembly assembly, CalcElement? element=null, string elementGroup=null)
        {
            var snapshots = new List<AssemblySnapshot>();
            foreach (var component in assembly.CalculationComponents)
            {
                var snapshot = CreateAssemblySnapshot(component, assembly);

                // claim element in calc project
                if (element != null && elementGroup != null)
                {
                    snapshot.ClaimElement(element.Value, elementGroup);
                }
                snapshots.Add(snapshot);
            }
            return snapshots;
        }

        /// <summary>
        /// Creates an assembly snapshot from a single calculation component (represents one material layer) in an assembly.
        /// In calc builder, the element type id exists in the calculation component,
        /// in calc project, assembly are generated from storage driver on the fly, thus element type id should be null.
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
            aSnapshot.AssignMaterialSnapshot(materialSnapshot, caComponent.ElementTypeId);
            return aSnapshot;
        }

        /// <summary>
        /// Creates a merged list of snapshots from the raw list.
        /// </summary>
        public static List<AssemblySnapshot> MergeAssemblySnapshots(List<AssemblySnapshot> assemblySnapshots)
        {
            var result = new List<AssemblySnapshot>();

            foreach (var aSnapshot in assemblySnapshots)
            {
                var existingSnapshot = result.Find(s => s.Equals(aSnapshot));
                if (existingSnapshot == null)
                {
                    result.Add(aSnapshot.Copy());
                }
                else
                {
                    existingSnapshot.Merge(aSnapshot);
                }
            }

            return result;
        }

       

    }
}
