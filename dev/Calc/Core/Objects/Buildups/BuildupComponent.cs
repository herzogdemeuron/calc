using Calc.Core.Calculation;
using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.Core.Objects.Buildups

{
    /// <summary>
    /// All instances of a revit/rhino type in the builder group.
    /// Includes all the layers and basic parameters.
    /// </summary>
    public class BuildupComponent : INotifyPropertyChanged, ICalcComponent
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        public int TypeIdentifier { get; set; } // revit type id
        private bool isNormalizer;
        [JsonProperty("is_normalizer")]
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
        [JsonProperty("layers")]
        public List<LayerComponent> LayerComponents { get; set; }
        public HslColor HslColor { get => ItemPainter.DefaultColor; }
        public bool HasLayers => LayerComponents.Count > 0;

        public void UpdateCalculationComponents(double normalizeRatio)
        {
            foreach (var layer in LayerComponents)
            {
                layer.UpdateCalculation(normalizeRatio);
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
        public object SerializeRecord()
        {
            return new
            {
                name = Name,
                is_normalizer = IsNormalizer,
                layers = LayerComponents.Select(l => l.SerializeRecord()).ToList()
            };
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
