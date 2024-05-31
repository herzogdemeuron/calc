using Calc.Core.Color;
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
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("function")]
        public MaterialFunction Function { get; set; }
        [JsonProperty("amount")]
        public double? Amount { get; set; }
        [JsonProperty("carbon_a1a3")]
        public double? Gwp { get; set; }
        [JsonProperty("grey_energy_fabrication_total")]
        public double? Ge { get; set; }
        [JsonProperty("cost")]
        public double? Cost { get; set; }

        [JsonProperty("calc_materials_id")]
        public Material Material { get; set; }
        public HslColor HslColor { get; set; }
        public bool HasError { get; set; }
        public bool IsComplete { get => CheckComplete(); }

        public void LinkMaterial(List<Material> materials)
        {
            if (Material != null)
            {
                Material = materials.Find(m => m.Id == Material.Id);
            }
        }

        public static List<CalculationComponent> FromLayer(LayerComponent layer, double totalRatio)
        {
            var result = new List<CalculationComponent>();
   
            if (!layer.HasMainMaterial) return result;
            var mainCalculationComponent = FromLayerMaterial(layer, totalRatio, layer.HslColor, true);
            result.Add(mainCalculationComponent);

            if (!layer.HasSubMaterial) return result;
            var subCalculationComponent = FromLayerMaterial(layer, totalRatio, layer.HslColor, false);
            result.Add(subCalculationComponent);
            
            return result;
        }

       public static CalculationComponent FromLayerMaterial(LayerComponent layer, double totalRatio, HslColor hslColor, bool getMain = true)
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
                Amount = (layerAmount * materialRatio).Value,
                HasError = layerAmountParam.HasError,
                Gwp = Math.Round(materialGwp.Value,3),
                Ge = Math.Round(materialGe.Value,3),
                HslColor = hslColor
            };
        }

        public bool CheckComplete()
        {
            return Function != null && Amount != null && Amount > 0;
        }

        /// <summary>
        /// update the position number of the components
        /// </summary>
        /// <param name="components"></param>
        public static void UpdatePosition(List<CalculationComponent> components)
        {
            if (components == null) return;
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Position = i + 1;
            }
        }

    }
}
