using Calc.Core.Objects.Assemblies;
using System.Collections.Generic;

namespace Calc.Core.Objects.Elements
{
    /// <summary>
    /// Used by the calc builder,
    /// holds the result of the element source selection,
    /// including the metadata and the transformed assembly components.
    /// </summary>
    public class ElementSourceSelectionResult
    {
        public string AssemblyCode { get; set; }
        public string AssemblyName { get; set; }
        public Unit AssemblyUnit { get; set; }
        public AssemblyGroup AssemblyGroup { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<AssemblyComponent> AssemblyComponents { get; set; }

        /// <summary>
        /// Apply the assembly record to current assembly components, so that they are mapped with the same materials.
        /// Assembly code, parameters and assembly components are alread at hand in the raw selection result,
        /// matches the recorded components to the current components with the same name and same layers.
        /// duplicate component names would be ignored to avoid misunderstanding.
        /// </summary>
        public void ApplyAssemblyRecord(AssemblyRecord record, CalcStore store)
        {
            if (record == null) return;

            AssemblyName = record.AssemblyName;
            Description = record.Description;
            AssemblyUnit = record.AssemblyUnit;
            AssemblyGroup = store.AssemblyGroupsAll.Find(g => g.Id == record.AssemblyGroup.Id);

            if (AssemblyComponents == null || AssemblyComponents.Count == 0) return;

            // link materials to the record components
            record.Components.ForEach(component => LinkMaterials(component, store));

            foreach ( var currentComponent in AssemblyComponents) 
            {
                var currentName = currentComponent.Name;
                if (AssemblyComponents.FindAll(c => c.Name == currentName).Count > 1) continue;

                var recordComponent = record.Components.Find(c => CheckComponentEquality(currentComponent, c));
                if (recordComponent == null) continue;
                FollowComponent(currentComponent, recordComponent);
            }
        }

        /// <summary>
        /// Checks if two assembly components have the same name and the same layers,
        /// the target material and thickness must be identical to be the same layer.
        /// </summary>
        private bool CheckComponentEquality(AssemblyComponent currentComponent, AssemblyComponent recordComponent)
        {
            if (currentComponent.Name != recordComponent.Name) return false;
            if (currentComponent.LayerComponents.Count != recordComponent.LayerComponents.Count) return false;

            for (int i = 0; i < currentComponent.LayerComponents.Count; i++)
            {
                var currentLayer = currentComponent.LayerComponents[i];
                var recordLayer = recordComponent.LayerComponents[i];
                if (currentLayer.TargetMaterialName != recordLayer.TargetMaterialName) return false;
                if (currentLayer.Thickness != recordLayer.Thickness) return false;
            }
            return true;            
        }

        /// <summary>
        /// Applies one assembly component from the record to the current component,
        /// applying isnormalizer and the materials of layers.
        /// </summary>
        private void FollowComponent(AssemblyComponent currentComponent, AssemblyComponent recordComponent)
        {
            currentComponent.IsNormalizer = recordComponent.IsNormalizer;
            var recordLayers = recordComponent.LayerComponents;

            if (currentComponent.LayerComponents == null) return;
            if (recordComponent.LayerComponents.Count != currentComponent.LayerComponents.Count) return;

            for (int i = 0; i < currentComponent.LayerComponents.Count; i++)
            {
                var currentLayer = currentComponent.LayerComponents[i];
                var recordLayer = recordLayers[i];
                FollowLayer(currentLayer, recordLayer);
            }
        }
        
        /// <summary>
        /// Copies data from the record layer to the current layer.
        /// </summary>
        private void FollowLayer(LayerComponent currentLayer, LayerComponent recordLayer)
        {
            currentLayer.Function = recordLayer.Function;
            currentLayer.MainMaterial = recordLayer.MainMaterial;
            currentLayer.SubMaterial = recordLayer.SubMaterial;
            currentLayer.SubMaterialRatio = recordLayer.SubMaterialRatio;
        }

        /// <summary>
        /// Links the material data from the calc store to the assembly component using material id.
        /// </summary>
        private void LinkMaterials(AssemblyComponent assemblyComponent, CalcStore store)
        {
            if (assemblyComponent.LayerComponents == null) return;
            foreach (var layer in assemblyComponent.LayerComponents)
            {
                if(layer.Function != null)
                {
                    layer.Function = store.MaterialFunctionsAll.Find(f => f.Id == layer.Function.Id);
                }

                if(layer.MainMaterial == null) continue;
                layer.MainMaterial = store.MaterialsAll.Find(m => m.Id == layer.MainMaterial.Id);

                if(layer.SubMaterial == null) continue;
                layer.SubMaterial = store.MaterialsAll.Find(m => m.Id == layer.SubMaterial.Id);
            }
        }
    }
}
