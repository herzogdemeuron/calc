using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Calc.Core.Calculations;
using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;

namespace Calc.Core.Objects.Buildups
{
    /// <summary>
    /// A layer or component of an element type in revit/rhino.
    /// A LayerComponent must have mapped infos，could have Ids to be targeted.
    /// </summary>
    public class LayerComponent : ICalcComponent, IColorizable, INotifyPropertyChanged
    {
        // general properties
        public bool IsValid { get; set; } = true;
        public string Title { get => TargetMaterialName?? "No Material"; }
        public string TargetMaterialName { get; }
        public double? TargetThickness { get; }
        public MaterialFunction? Function { get; set; }
        public BasicParameterSet BasicParameterSet { get; set; }

        // calculation components
        public List<CalculationComponent> CalculationComponents { get; set; } = new List<CalculationComponent>();
        public bool HasParamError { get => CalculationComponents.Exists(c => c.HasError); }
        public bool CalculationCompleted { get => CalculationComponents.TrueForAll(c => c.IsComplete) && MaterialUnitsMatch; }
        private bool MaterialUnitsMatch { get => CheckUnitsMatch(); }

        // material mappings
        public Material MainMaterial { get; set; }
        public Material SubMaterial { get; set; }
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

        public LayerComponent(string materialName, BasicParameterSet basicParameterSet = null, double? thickness = null)
        {
            TargetMaterialName = materialName;
            BasicParameterSet = basicParameterSet;
            TargetThickness = thickness;
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

        private string GetColorIdentifier()
        {
            var result = "";
            if (!HasMainMaterial) return result;
            result += MainMaterial.Id;
            if (HasSubMaterial) result += $"-{SubMaterial.Id}";
            return result;
        }

        public double GetMaterialGwp(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var ratio = getMain ? MainMaterialRatio : SubMaterialRatio;
                var gwp = material.Gwp ?? 0;
                return gwp * ratio;
            };
            return 0;
        }

        public double GetMaterialGe(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var ratio = getMain ? MainMaterialRatio : SubMaterialRatio;
                var ge = material.Ge ?? 0;
                return ge * ratio;
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

        public void UpdateCalculation(double totalRation)
        {
            CalculationComponents = CalculationComponent.FromLayer(this, totalRation);
            OnPropertyChanged(nameof(HasParamError));
            OnPropertyChanged(nameof(CalculationCompleted));

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
