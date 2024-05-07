using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// showing the basic amounts of a ICalcComponent
    /// </summary>
    public class AmountModel : INotifyPropertyChanged
    {
        private readonly Unit unit;
        private ICalcComponent host;
        private Unit? buildupUnit;
        private Unit? materialUnit;
        private BasicParameterSet basicParameterSet { get => host?.BasicParameterSet; }
        private bool HasError { get => (basicParameterSet?.GetAmountParam(unit)?.HasError) ?? true; }
        private bool IsHostNormalizable { get => host is BuildupComponent; }
        public string AmountString { get => GetAmountString(); } // show the sting of the basic amounts in UI 
        public bool IsNormalizable { get => CheckNormalizable(); }  // show if each amount can be set as a normalizer
        public bool IsNormalizer { get => CheckNormalizer(); } // show if an amount is set as a normalizer
        public bool IsUsed { get => CheckUsed(); }  // show if an amount is used for normalization or for material calculation
        public bool WarningActive { get => IsUsed && HasError; }
        public AmountModel(Unit unit)
        {
            this.unit = unit;
        }

        /// <summary>
        /// Each time the component selection, the Buildup Unit or the Material selection is changed, the BasicUnitModel needs to be updated.
        /// </summary>
        public void UpdateWithHost(ICalcComponent host, Unit? buildupUnit, Unit? materialUnit)
        {
            this.host = host;
            this.buildupUnit = buildupUnit;
            this.materialUnit = materialUnit;
            NotifyUpdate();
        }

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

        private bool CheckNormalizable()
        {
            if (!IsHostNormalizable) return false;
            var amountParam = basicParameterSet?.GetAmountParam(unit);
            if (amountParam == null) return false;
            if (amountParam.Amount == null) return false;
            return amountParam.Amount > 0;
        }

        private bool CheckNormalizer()
        {
            if (!IsHostNormalizable) return false;
            var bc = host as BuildupComponent;
            return bc.IsNormalizer && this.buildupUnit == unit;
        }

        private bool CheckUsed()
        {
            switch (IsHostNormalizable)
            {
                case true:
                    return (host as BuildupComponent).IsNormalizer && this.buildupUnit == unit;
                case false:
                    return this.materialUnit == unit;
                default:
                    return false;
            }
        }
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
