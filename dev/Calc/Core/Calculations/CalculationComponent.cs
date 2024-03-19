using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Results;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calc.Core.Calculations
{
    /// <summary>
    /// A component used in calculation.
    /// stands for the amount of a material layer
    /// </summary>
    public class CalculationComponent
    {
        [JsonProperty("function")]
        public MaterialFunction Function { get; set; }
        [JsonProperty("amount")]
        public double? Amount { get; set; }
        [JsonProperty("gwp")]
        public double? Gwp { get; set; }
        [JsonProperty("ge")]
        public double? Ge { get; set; }
        [JsonProperty("cost")]
        public double? Cost { get; set; }

        [JsonProperty("calc_builder_materials_id")]
        public Material Material { get; set; }
        public Unit MaterialUnit { get; set; }
        public bool HasError { get; set; }

        public void LinkMaterial(List<Material> materials)
        {
            if (Material != null)
            {
                Material = materials.Find(m => m.Id == Material.Id);
            }
        }

        public static List<CalculationComponent> FromLayerComponents(List<LayerComponent> layerComponents, double totalRatio)
        {
            var result = new List<CalculationComponent>();
            foreach (var layer in layerComponents)
            {

                if (!layer.HasMainMaterial) continue;
                var mainCalculationComponent = FromLayerComponent(layer, totalRatio, true);
                result.Add(mainCalculationComponent);

                if (!layer.HasSubMaterial) continue;
                var subCalculationComponent = FromLayerComponent(layer, totalRatio, false);
                result.Add(subCalculationComponent);
            }
            return result;
        }

       private static CalculationComponent FromLayerComponent(LayerComponent layer, double totalRatio, bool getMain = true)
        {
            var layerAmountParam = layer.GetAmountParam();
            var layerAmount = (layerAmountParam?.Amount != null) ? layerAmountParam.Amount * totalRatio : 0;
            var materialRatio = getMain ? layer.MainMaterialRatio : layer.SubMaterialRatio;

            var materialGwp = layer.GetMaterialGwp(getMain) * layerAmount;
            var materialGe = layer.GetMaterialGe(getMain) * layerAmount;

            return new CalculationComponent
            {
                Material = getMain ? layer.MainMaterial : layer.SubMaterial,
                Function = layer.Function,
                Amount = layerAmount * materialRatio,
                HasError = layerAmountParam.HasError,
                Gwp = materialGwp,
                Ge = materialGe
            };
        }

    }
}
