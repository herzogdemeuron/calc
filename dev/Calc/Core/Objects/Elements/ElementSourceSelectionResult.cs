using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Elements
{
    public class ElementSourceSelectionResult
    {
        public string BuildupCode { get; set; }
        public string BuildupName { get; set; }
        public BuildupGroup BuildupGroup { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<BuildupComponent> BuildupComponents { get; set; }

        /// <summary>
        /// apply the buildup record to current result, so that the buildup components are mapped with the same materials
        /// buildup code, parameters and buildup components are alread at hand in the raw selection result
        /// apply the components from the record to the current component with the same name
        /// apply the record components to the current components with the same order
        /// </summary>
        public void ApplyBuildupRecord(BuildupRecord record, CalcStore store)
        {
            if (record == null) return;

            BuildupName = record.BuildupName;
            Description = record.Description;
            BuildupGroup = store.BuildupGroupsAll.Find(g => g.Id == record.BuildupGroup.Id);

            if (BuildupComponents == null || BuildupComponents.Count == 0) return;

            // link materials to the record components
            var recordComponents = new List<BuildupComponent>(record.Components);
            recordComponents.ForEach(component => LinkMaterials(component, store));

            foreach ( var currentComponent in BuildupComponents) 
            {
                var recordComponent = recordComponents.Find(c => c.Name == currentComponent.Name);
                if (recordComponent == null) continue;
                ApplyComponent(currentComponent, recordComponent);
                recordComponents.Remove(recordComponent);
            }
        }

        /// <summary>
        /// apply one component from the record to the current component
        /// by applying isnormalizer and the materials of layers
        /// layers must have the same target material
        /// </summary>
        private void ApplyComponent(BuildupComponent currentComponent, BuildupComponent recordComponent)
        {
            currentComponent.IsNormalizer = recordComponent.IsNormalizer;
            var recordLayers = new List<LayerComponent>(recordComponent.LayerComponents);

            if (currentComponent.LayerComponents == null) return;
            foreach (var currentLayer in currentComponent.LayerComponents)
            {
                var recordLayer = recordLayers.Find(l => l.TargetMaterialName == currentLayer.TargetMaterialName);
                if (recordLayer == null) continue;
                ApplyLayer(currentLayer, recordLayer);
                recordLayers.Remove(recordLayer);
            }
        }
        
        private void ApplyLayer(LayerComponent currentLayer, LayerComponent recordLayer)
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
