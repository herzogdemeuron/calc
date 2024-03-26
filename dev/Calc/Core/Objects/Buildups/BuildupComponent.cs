using Calc.Core.Calculations;
using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Calc.Core.Objects.Buildups

{
    /// <summary>
    /// All instances of a revit/rhino type in the builder group.
    /// </summary>
    public class BuildupComponent : INotifyPropertyChanged, ICalcComponent
    {
        public string Title { get; set; }
        public int TypeIdentifier { get; set; } // revit type id
        private bool isNormalizer;
        public bool IsNormalizer
        {
            get => isNormalizer;
            set
            {
                isNormalizer = value;
                OnPropertyChanged(nameof(IsNormalizer));
            }
        }

        private bool hasParamError;
        public bool HasParamError
        {
            get => hasParamError;
            set
            {
                hasParamError = value;
                OnPropertyChanged(nameof(HasParamError));
            }
        }

        public List<int> ElementIds { get; set; }
        public bool IsCompoundElement { get; set; }
        public double? Thickness { get; set; }
        public BasicParameterSet BasicParameterSet { get; set; }
        public List<LayerComponent> LayerComponents { get; set; }
        public HslColor HslColor { get => ItemPainter.DefaultColor; }

        public bool HasLayers => LayerComponents.Count > 0;

        public void UpdateCalculationComponents(double totalRation)
        {
            foreach (var layer in LayerComponents)
            {
                layer.UpdateCalculation(totalRation);
            }
        }

        public List<CalculationComponent> GetCalculationComponents()
        {
            return LayerComponents.Where(l => l.CalculationCompleted).SelectMany(l => l.CalculationComponents).ToList();
        }

        public void UpdateParamError(Unit unit)
        {
            HasParamError = BasicParameterSet?.GetAmountParam(unit)?.HasError ?? true;
        }

        public bool Equals(BuildupComponent component)
        {
            return TypeIdentifier == component.TypeIdentifier;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
