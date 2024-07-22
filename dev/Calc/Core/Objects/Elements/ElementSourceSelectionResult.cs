using Calc.Core.Objects.Buildups;
using System.Collections.Generic;

namespace Calc.Core.Objects.Elements
{
    public class ElementSourceSelectionResult
    {
        public string BuildupCode { get; set; }
        public string BuildupName { get; set; }
        public Unit BuildupUnit { get; set; }
        public BuildupGroup BuildupGroup { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<BuildupComponent> BuildupComponents { get; set; }

        /// <summary>
        /// apply the buildup record to current result, so that the buildup components are mapped with the same materials
        /// buildup code, parameters and buildup components are alread at hand in the raw selection result
        /// apply the components from the record to the current component with the same name and same layers
        /// duplicate current component names would be ignored to avoid confusion
        /// </summary>
        public void ApplyBuildupRecord(BuildupRecord record, CalcStore store)
        {
            if (record == null) return;

            BuildupName = record.BuildupName;
            Description = record.Description;
            BuildupUnit = record.BuildupUnit;
            BuildupGroup = store.BuildupGroupsAll.Find(g => g.Id == record.BuildupGroup.Id);

            if (BuildupComponents == null || BuildupComponents.Count == 0) return;

            // link materials to the record components
            record.Components.ForEach(component => LinkMaterials(component, store));

            foreach ( var currentComponent in BuildupComponents) 
            {
                var currentName = currentComponent.Name;
                if (BuildupComponents.FindAll(c => c.Name == currentName).Count > 1) continue;

                var recordComponent = record.Components.Find(c => CheckComponentEquality(currentComponent, c));
                if (recordComponent == null) continue;
                FollowComponent(currentComponent, recordComponent);
            }
        }

        /// <summary>
        /// check if both components have the same name and the same layers
        /// the target material and thickness must be identical to be the same layer
        /// </summary>
        private bool CheckComponentEquality(BuildupComponent currentComponent, BuildupComponent recordComponent)
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
        /// apply one component from the record to the current component
        /// by applying isnormalizer and the materials of layers
        /// layers must have the same target material
        /// </summary>
        private void FollowComponent(BuildupComponent currentComponent, BuildupComponent recordComponent)
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
        
        private void FollowLayer(LayerComponent currentLayer, LayerComponent recordLayer)
        {
            currentLayer.Function = recordLayer.Function;
            currentLayer.MainMaterial = recordLayer.MainMaterial;
            currentLayer.SubMaterial = recordLayer.SubMaterial;
            currentLayer.SubMaterialRatio = recordLayer.SubMaterialRatio;
        }

        private void LinkMaterials(BuildupComponent buildupComponent, CalcStore store)
        {
            if (buildupComponent.LayerComponents == null) return;
            foreach (var layer in buildupComponent.LayerComponents)
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
