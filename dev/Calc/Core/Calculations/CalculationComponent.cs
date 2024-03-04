using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
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
        public Material Material { get; set; }
        public double Amount { get; set; }
        public Unit MaterialUnit { get; set; }
        public bool HasError { get; set; }
        public double GWP { get; set; }
        public double GE { get; set; }
        public double Cost { get; set; }

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
            var unit = materialComponent.SelectedDensity.Unit;
            var layerQuantityParam = layer.GetQuantityParam(unit);
            var hasError = layerQuantityParam.ErrorType != null;
            var layerQuantity = layerQuantityParam.Value ?? 0;
            layerQuantity = layerQuantity * totalRatio;

            var amount = materialComponent.AmountPerUnit * layerQuantity;
            var materialGwp = materialComponent.Material.GWP;
            var materialGe = materialComponent.Material.GE;

            return new CalculationComponent
            {
                Material = materialComponent.Material,
                Amount = amount,
                MaterialUnit = unit,
                HasError = hasError,
                GWP = materialGwp * amount,
                GE = materialGe * amount,
            };
        }

    }
}
