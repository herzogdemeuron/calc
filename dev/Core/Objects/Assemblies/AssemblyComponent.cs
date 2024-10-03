using Calc.Core.Calculation;
using Calc.Core.Color;
using Calc.Core.Objects.BasicParameters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// Used in the calc builder, representing one selected type from revit.
    /// Includes all instances of a revit/rhino type in the builder group,
    /// as well as all the layers (LayerComponent) and basic parameters of each type.
    /// </summary>
    public class AssemblyComponent : INotifyPropertyChanged, ICalcComponent
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

        /// <summary>
        /// Update the calculation components of all layer components.
        /// </summary>
        public void UpdateCalculationComponents(double normalizeRatio)
        {
            foreach (var layer in LayerComponents)
            {
                layer.UpdateCalculation(normalizeRatio);
            }
        }

        /// <summary>
        /// Collect the calculation components from all layer components.
        /// </summary>
        public List<CalculationComponent> GetCalculationComponents()
        {
            return LayerComponents.Where(l => l.CalculationCompleted).SelectMany(l => l.CalculationComponents).ToList();
        }

        /// <summary>
        /// Check if there are errors in the basic parameters
        /// </summary>
        public void UpdateParamError(Unit unit)
        {
            HasParamError = BasicParameterSet?.GetAmountParam(unit)?.HasError ?? true;
        }


        /// <summary>
        /// Serializes the assembly component with layers.
        /// </summary>
        internal object SerializeRecord()
        {
            return new
            {
                name = Name,
                is_normalizer = IsNormalizer,
                layers = LayerComponents
                .Select(l => l.SerializeRecord())
                .ToList()
            };
        }
        public bool Equals(AssemblyComponent component)
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
