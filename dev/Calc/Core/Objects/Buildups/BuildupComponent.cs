using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calc.Core.Objects.Buildups

{
    /// <summary>
    /// All instances of a revit/rhino type in the builder group.
    /// </summary>
    public class BuildupComponent
    {
        public List<int> Ids { get; set; }
        public string TypeIdentifier { get; set; }
        public List<LayerComponent> LayerComponents { get; set; } = new List<LayerComponent>();
        public bool IsNormalizer { get; set; }
        public BasicParameterSet TotalBasicParameterSet { get; set; }
        public bool HasLayers => LayerComponents.Count > 0;
        public bool CheckSource(BuildupComponent buildupComponent)
        {
            return this.TypeIdentifier == buildupComponent.TypeIdentifier;
        }

        /// <summary>
        /// apply a source buildupComponent to current revit/rhino buildupComponent, 
        /// check if its layerComponents match the current ones, 
        /// if true, set this layerComponent with the source layerComponent,
        /// else it stays in the list and will be presented as a unmapped layer.
        /// return a new buildupComponent with the missing source layerComponents.
        /// </summary>
        public BuildupComponent ApplySource(BuildupComponent sourceBuildupComponent)
        {

            this.IsNormalizer = sourceBuildupComponent.IsNormalizer;
            var sourceLayers = sourceBuildupComponent.LayerComponents;

            foreach (LayerComponent newLayer in LayerComponents)
            {
                var sourceLayer = sourceLayers.FirstOrDefault(l => l.CheckSource(newLayer));

                if (sourceLayer != null)
                {
                    newLayer.ApplySource(sourceLayer);
                    sourceLayers.Remove(sourceLayer);
                }
            }
            return new BuildupComponent { TypeIdentifier = this.TypeIdentifier, LayerComponents = sourceLayers };
        }
    }
}
