using Calc.Core.Color;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Calc.Core.Calculation
{
    /// <summary>
    /// The calculation of a material layer in the assembly, amount per assembly unit.
    /// </summary>
    public class CalculationComponent
    {
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("function")]
        public MaterialFunction Function { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("carbon_a1a3")]
        public double? Gwp { get; set; } // calculated gwp
        [JsonProperty("grey_energy_fabrication_total")]
        public double? Ge { get; set; } // calculated ge
        [JsonProperty("cost")]
        public double? Cost { get; set; }
        [JsonProperty("calc_materials_id")]
        public Material Material { get; set; }
        [JsonIgnore]
        public string ElementTypeId { get; set; }
        public HslColor HslColor { get; set; }
        public bool HasError { get; set; }
        public bool IsComplete { get => CheckComplete(); }

        /// <summary>
        /// Find and replace the material from the list with the same id
        /// </summary>
        internal void LinkMaterial(List<Material> materials)
        {
            if (Material != null)
            {
                Material = materials.Find(m => m.Id == Material.Id);
            }
        }

        /// <summary>
        /// Creates the calculation component from a layer component
        /// </summary>
        internal static List<CalculationComponent> FromLayer(LayerComponent layer, double normalizeRatio)
        {
            var result = new List<CalculationComponent>();

            if (!layer.HasMainMaterial) return result;
            var mainCalculationComponent = FromLayerMaterial(layer, normalizeRatio, true);
            result.Add(mainCalculationComponent);

            if (!layer.HasSubMaterial) return result;
            var subCalculationComponent = FromLayerMaterial(layer, normalizeRatio, false);
            result.Add(subCalculationComponent);
            
            return result;
        }

        /// <summary>
        /// Creates a calculation component from the layer material.
        /// </summary>
        private static CalculationComponent FromLayerMaterial(LayerComponent layer, double normalizeRatio, bool isMain = true)
        {
            var layerAmountParam = layer.GetAmountParam();
            var layerAmount = layer.GetLayerAmount(normalizeRatio, isMain);
            var amortizationFactor = layer.GetAmortizationFactor();
            var calculatedGwp = layer.GetMaterialGwp(isMain) * layerAmount * amortizationFactor;
            var calculatedGe = layer.GetMaterialGe(isMain) * layerAmount * amortizationFactor;

            return new CalculationComponent
            {
                Material = isMain ? layer.MainMaterial : layer.SubMaterial,
                Function = layer.Function,
                Amount = Math.Round(layerAmount.Value, 5),
                HasError = layerAmountParam.HasError,
                Gwp = Math.Round(calculatedGwp.Value,5),
                Ge = Math.Round(calculatedGe.Value,5),
                ElementTypeId = layer.TypeIdentifier,
                HslColor = layer.HslColor
            };
        }

        /// <summary>
        /// Checks if the calculation can be completed, function and amount must be valid.
        /// </summary>
        private bool CheckComplete()
        {
            return Function != null && Amount > 0;
        }

        /// <summary>
        /// Update the position number of the components.
        /// </summary>
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
