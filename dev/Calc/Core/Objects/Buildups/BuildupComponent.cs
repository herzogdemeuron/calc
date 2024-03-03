using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Buildups
{
    public class BuildupComponent
    {
        public MaterialComponentSet MaterialComponentSet { get; set; }

        public LayerComponent LayerComponent { get; set; }

        public bool IsNormalizer { get; set; }
        public bool IsMissing { get => LayerComponent == null; }

        public bool CheckTarget(LayerComponent layerComponent)
        {
            return LayerComponent.CheckTarget(layerComponent);
        }

        public void SetTarget(LayerComponent layerComponent)
        {
            LayerComponent.SetTarget(layerComponent);
        }

        public void SetMainMaterial(Material material)
        {
            MaterialComponentSet.SetMainMaterial(material);
        }

        public void SetSubMaterial(Material material)
        {
            MaterialComponentSet.SetSubMaterial(material);
        }
    }
}
