using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
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
        public string Function { get; set; }
        [JsonProperty("quantity")]
        public double Quantity { get; set; }
        [JsonProperty("gwp")]
        public double GWP { get; set; }
        [JsonProperty("ge")]
        public double GE { get; set; }
        [JsonProperty("cost")]
        public double Cost { get; set; }
        [JsonProperty("calc_builder_materials_id")]
        public Material Material { get; set; }
        public Unit MaterialUnit { get => Material.MaterialUnit; }
        public bool HasError { get; set; }

        public static List<CalculationComponent> FromBuildupComponent(BuildupComponent buildupComponent, double totalRatio)
        {
            var result = new List<CalculationComponent>();
            foreach (var layer in buildupComponent.LayerComponents)
            {
                var materialSet = layer.MaterialComponentSet;

                if (!materialSet.HasMainMaterial) continue;
                var mainCalculationComponent = FromMaterialComponent(materialSet.MainMaterialComponent, layer, totalRatio);
                result.Add(mainCalculationComponent);

                if (!materialSet.HasSubMaterial) continue;
                var subCalculationComponent = FromMaterialComponent(materialSet.SubMaterialComponent, layer, totalRatio);
                result.Add(subCalculationComponent);
            }
            return result;
        }

       private static CalculationComponent FromMaterialComponent(MaterialComponent materialComponent, LayerComponent layer, double totalRatio)
        {
            var unit = materialComponent.Density.Unit;
            var layerAmountParam = layer.GetAmountParam(unit);
            var hasError = layerAmountParam.ErrorType != null;
            var layerAmount = layerAmountParam.Amount ?? 0;
            layerAmount *= totalRatio;

            var quantity = materialComponent.QuantityPerUnit * layerAmount;
            var materialGwp = materialComponent.Material.GWP;
            var materialGe = materialComponent.Material.GE;

            return new CalculationComponent
            {
                Material = materialComponent.Material,
                Function = materialComponent.Function,
                Quantity = quantity,
                HasError = hasError,
                GWP = materialGwp * quantity,
                GE = materialGe * quantity,
            };
        }

    }
}
