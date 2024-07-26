using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Calc.Core.Calculation;
using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;
using Newtonsoft.Json;

namespace Calc.Core.Objects.Buildups
{
    /// <summary>
    /// For calc builder.
    /// A layer or component of an element type in revit/rhino.
    /// A LayerComponent must have mapped infos，could have Ids to be targeted.
    /// </summary>
    public class LayerComponent : ICalcComponent, IColorizable, INotifyPropertyChanged
    {
        // general properties
        public string TypeIdentifier { get; set; } // revit type id
        public bool IsValid { get; set; } = true;
        public string Name { get => TargetMaterialName?? "No Material"; }
        [JsonProperty("target_material_name")]
        public string TargetMaterialName { get; set; }
        [JsonProperty("thickness")]
        public double? Thickness { get; set; }
        [JsonProperty("function")]
        public MaterialFunction Function { get; set; }
        public BasicParameterSet BasicParameterSet { get; set; }

        // calculation components
        public List<CalculationComponent> CalculationComponents { get; set; } = new();
        public bool HasParamError { get => CalculationComponents.Exists(c => c.HasError); }
        public bool CalculationCompleted { get => CalculationComponents.TrueForAll(c => c.IsComplete) && MaterialUnitsMatch; }
        private bool MaterialUnitsMatch { get => CheckUnitsMatch(); }

        // material mappings
        private Material mainMaterial;
        [JsonProperty("main_material")]
        public Material MainMaterial
        {
            get => mainMaterial;
            set
            {
                mainMaterial = value;
                OnPropertyChanged(nameof(MainMaterial));
            }
        }
        private Material subMaterial;
        [JsonProperty("sub_material")]
        public Material SubMaterial
            {
            get => subMaterial;
            set
            {
                subMaterial = value;
                OnPropertyChanged(nameof(SubMaterial));
            }
        }
        [JsonProperty("sub_material_ratio")]
        public double SubMaterialRatio { get; set; } = 0;
        public double MainMaterialRatio { get => 1 - SubMaterialRatio; }
        public bool HasMainMaterial { get => MainMaterial != null; }
        public bool HasSubMaterial { get => HasMainMaterial && SubMaterial != null; }
        public string ColorIdentifier{ get => GetColorIdentifier(); }

        private HslColor hslColor;
        public HslColor HslColor
        {
            get => hslColor;
            set
            {
                hslColor = value;
                OnPropertyChanged(nameof(HslColor));
            }
        }

        public LayerComponent(){}

        public bool Equals(LayerComponent component)
        {
            return TargetMaterialName == component.TargetMaterialName;
        }

        public void SetMainMaterial(Material material)
        {
            MainMaterial = material;
        }

        public void SetSubMaterial(Material material)
        {
            SubMaterial = material;
        }

        public void SetSubMaterialRatio(double ratio)
        {
            SubMaterialRatio = ratio;
        }

        /// <summary>
        /// get the amount parameter of this layer using the unit from the main material
        /// </summary>
        public BasicParameter GetAmountParam()
        {
            if (BasicParameterSet == null) return null;
            var unit = MainMaterial?.MaterialUnit;
            if (unit == null) return null;
            return BasicParameterSet.GetAmountParam((Unit)unit);
        }

        /// <summary>
        /// get the real amount of the main/sub material in the layer, normalized
        /// </summary>
        public double? GetLayerAmount(double normalizeRatio, bool getMain)
        {
            var ratio = getMain ? MainMaterialRatio : SubMaterialRatio;
            var layerAmountParam = GetAmountParam();
            var layerAmount = (layerAmountParam?.Amount != null) ? layerAmountParam.Amount * normalizeRatio * ratio : 0;
            return layerAmount;
        }

        public double GetAmortizationFactor()
        {
            var result = Function?.Amortization ?? 0;
            if (result == 0) return 0;
            var factor = 1/(double)result;
            return factor;
        }

        private string GetColorIdentifier()
        {
            var result = "";
            if (!HasMainMaterial) return result;
            result += MainMaterial.Id;
            if (HasSubMaterial) result += $"-{SubMaterial.Id}";
            return result;
        }

        /// <summary>
        /// get the unit gwp of the main/sub material
        /// </summary>
        public double GetMaterialGwp(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var gwp = material.Gwp ?? 0;
                return gwp;
            };
            return 0;
        }

        /// <summary>
        /// get the unit ge of the main/sub material
        /// </summary>
        public double GetMaterialGe(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var ge = material.Ge ?? 0;
                return ge;
            };
            return 0;
        }

        public bool CheckUnitsMatch()
        {
            if (HasMainMaterial && HasSubMaterial && MainMaterial.MaterialUnit != SubMaterial.MaterialUnit)
            {
                return false;
            }
            return true;
        }

        public void UpdateCalculation(double normalizeRatio)
        {
            CalculationComponents.Clear();
            CalculationComponents = CalculationComponent.FromLayer(this, normalizeRatio);
            OnPropertyChanged(nameof(HasParamError));
            OnPropertyChanged(nameof(CalculationCompleted));

        }

        public object SerializeRecord()
        {
            // no material assigned
            if (MainMaterial == null)
            {
                return new
                {
                    target_material_name = TargetMaterialName,
                    thickness = Thickness
                };
            }

            // no sub material assigned
            if (SubMaterial == null)
            {
                return new
                {
                    target_material_name = TargetMaterialName,
                    thickness = Thickness,
                    function = new { id = Function?.Id },
                    main_material = new { id = MainMaterial?.Id }
                };
            }

            // both main and sub material assigned
             return new
            {
                target_material_name = TargetMaterialName,
                thickness = Thickness,
                function = new { id = Function?.Id },
                main_material = new { id = MainMaterial?.Id },
                sub_material = new { id = SubMaterial?.Id },
                sub_material_ratio = SubMaterialRatio,
            };

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
