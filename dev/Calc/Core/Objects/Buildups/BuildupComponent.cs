using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    public class BuildupComponent
    {
        public List<int> ContainerIds { get; set; }
        public string TargetTypeName { get; set; }
        public bool IsNormalizer { get; set; }
        public List<LayerComponent> LayerComponents { get; set; } = new List<LayerComponent>();

        public bool CheckTarget(BuildupComponent buildupComponent)
        {
            return this.TargetTypeName == buildupComponent.TargetTypeName;
        }

        /// <summary>
        /// apply a buildupComponent from revit/rhino, check if its layerComponents are targeted in this buildupComponent, 
        /// if true, set the corresponding layerComponent with the new layerComponent,
        /// else add the new layerComponent to this buildupComponent.
        /// Remove the untargeted layerComponents, return a new buildupComponent with them.
        /// </summary>
        public BuildupComponent ApplyTarget(BuildupComponent newBuildupComponent)
        {
            this.ContainerIds = newBuildupComponent.ContainerIds;
            var untargetedLayers = new List<LayerComponent>(LayerComponents);
            LayerComponents.Clear();

            foreach (LayerComponent newLayer in newBuildupComponent.LayerComponents)
            {
                var existingLayer = untargetedLayers.FirstOrDefault(currentLayer => currentLayer.CheckTarget(newLayer));

                if (existingLayer != null)
                {
                    newLayer.ApplyTarget(existingLayer);
                    untargetedLayers.Remove(existingLayer);
                }

                LayerComponents.Add(newLayer);
            }
            return new BuildupComponent { LayerComponents = untargetedLayers };
        }
    }
}
