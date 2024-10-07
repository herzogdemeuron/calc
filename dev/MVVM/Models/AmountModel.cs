using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.BasicParameters;
using System.ComponentModel;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// Shows one basic unit amount of an ICalcComponent.
    /// </summary>
    internal class AmountModel : INotifyPropertyChanged
    {
        private readonly Unit unit;
        private ICalcComponent host;
        private Unit? assemblyUnit;
        private Unit? materialUnit;
        private BasicParameterSet basicParameterSet { get => host?.BasicParameterSet; }
        private bool HasError { get => (basicParameterSet?.GetAmountParam(unit)?.HasError) ?? true; }
        private bool IsHostNormalizable { get => host is AssemblyComponent; }
        public string AmountString { get => GetAmountString(); }
        public bool IsNormalizable { get => CheckNormalizable(); }
        public bool IsNormalizer { get => CheckNormalizer(); }
        public bool IsUsed { get => CheckUsed(); }
        public bool WarningActive { get => IsUsed && HasError; }
        public AmountModel(Unit unit)
        {
            this.unit = unit;
        }

        /// <summary>
        /// Each time during component selection, the Assembly Unit or the Material selection is changed,
        /// the AmountModel needs to be updated.
        /// </summary>
        internal void UpdateWithHost(ICalcComponent host, Unit? assemblyUnit, Unit? materialUnit)
        {
            this.host = host;
            this.assemblyUnit = assemblyUnit;
            this.materialUnit = materialUnit;
            NotifyUpdate();
        }

        /// <summary>
        /// The sting of the basic amounts in UI 
        /// </summary>
        private string GetAmountString()
        {
            if (basicParameterSet == null) return "-";
            var param = basicParameterSet.GetAmountParam(unit);
            string unitSting;
            switch (unit)
            {
                case Unit.piece:
                    unitSting = "piece";
                    break;
                case Unit.m3:
                    unitSting = "m³";
                    break;
                case Unit.m2:
                    unitSting = "m²";
                    break;
                case Unit.m:
                    unitSting = "m";
                    break;
                default:
                    unitSting = "?";
                    break;
            }

            if(!param.HasError)
            {
                var v = param.Amount.ToString();
                return $"{v} {unitSting}";
            }
            switch (param.ErrorType)
            {
                case ParameterErrorType.Missing:
                    return "absent";
                case ParameterErrorType.Redundant:
                    return "duplicate";
                case ParameterErrorType.ZeroValue:
                    return "0";
                default:
                return "?";
            }
        }

        /// <summary>
        /// If each amount can be set as a normalizer.
        /// </summary>
        private bool CheckNormalizable()
        {
            if (!IsHostNormalizable) return false;
            var amountParam = basicParameterSet?.GetAmountParam(unit);
            if (amountParam == null) return false;
            if (amountParam.Amount == null) return false;
            return amountParam.Amount > 0;
        }

        /// <summary>
        /// If an amount is set as a normalizer.
        /// </summary>
        private bool CheckNormalizer()
        {
            if (!IsHostNormalizable) return false;
            var ac = host as AssemblyComponent;
            return ac.IsNormalizer && this.assemblyUnit == unit;
        }

        /// <summary>
        /// Shows if an amount is used for normalization or for material calculation.
        /// </summary>
        private bool CheckUsed()
        {
            switch (IsHostNormalizable)
            {
                case true:
                    return (host as AssemblyComponent).IsNormalizer && this.assemblyUnit == unit;
                case false:
                    return this.materialUnit == unit;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Notify all the properties changed.
        /// </summary>
        private void NotifyUpdate()
        {
            OnPropertyChanged(nameof(AmountString));
            OnPropertyChanged(nameof(IsNormalizable));
            OnPropertyChanged(nameof(IsNormalizer));
            OnPropertyChanged(nameof(IsUsed));
            OnPropertyChanged(nameof(WarningActive));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
