using Calc.Core;
using Calc.Core.Calculations;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.ViewModels
{

    public class BuildupCreationViewModel : INotifyPropertyChanged
    {

        private DirectusStore store;
        public List<LcaStandard> StandardsAll { get => store.StandardsAll; }
        public readonly List<Unit> BuildupUnitsAll = new List<Unit> { Unit.piece, Unit.m, Unit.m2, Unit.m3 };
        private IBuildupComponentCreator buildupComponentCreator;
        public ICalcComponent SelectedComponent{ get; set; }

        private LcaStandard selectedStandard;
        public LcaStandard SelectedStandard
        {
            get => selectedStandard;
            set
            {
                if (selectedStandard != value)
                {
                    selectedStandard = value;
                    OnPropertyChanged(nameof(SelectedStandard));
                }
            }
        }

        private Unit selectedBuildupUnit;
        public Unit SelectedBuildupUnit
        {
            get => selectedBuildupUnit;
            set
            {
                selectedBuildupUnit = value;
                OnPropertyChanged(nameof(SelectedBuildupUnit));
            }
        }

        private BuildupGroup selectedBuildupGroup;
        public BuildupGroup SelectedBuildupGroup
        {
            get => selectedBuildupGroup;
            set
            {
                selectedBuildupGroup = value;
                OnPropertyChanged(nameof(SelectedBuildupGroup));
            }
        }

        private List<BuildupComponent> buildupComponents = new List<BuildupComponent>();
        public List<BuildupComponent> BuildupComponents
        {
            get => buildupComponents;
            set => OnPropertyChanged(nameof(BuildupComponents));
        }

        private List<CalculationComponent> calculationComponents = new List<CalculationComponent>();
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => calculationComponents;
            set => OnPropertyChanged(nameof(CalculationComponents));
        }

        public string CountString { get => GetValueString(Unit.piece); }

        public string LengthString { get => GetValueString(Unit.m); }

        public string AreaString { get => GetValueString(Unit.m2); }

        public string VolumeString { get => GetValueString(Unit.m3); }

        public BuildupCreationViewModel(DirectusStore store, IBuildupComponentCreator bcCreator)
        {
            this.store = store;
            buildupComponentCreator = bcCreator;
        }

        public void UpdateCalculationComponents()
        {
            var calculationComponents = new List<CalculationComponent>();
            var quantityRatio = GetQuantityRatio();
            if (quantityRatio != 0)
            {
                foreach (var component in BuildupComponents)
                {
                    var calculationComponent = CalculationComponent.FromBuildupComponent(component, quantityRatio);
                    calculationComponents.AddRange(calculationComponent);
                }
            }
            CalculationComponents = calculationComponents;
        }

        private string GetValueString(Unit unit)
        {
            return SelectedComponent.BasicParameterSet.GetAmountParam(unit).Amount?.ToString() ?? "?";
        }

        private double GetQuantityRatio()
        {
            var normalizer = BuildupComponents.Where(c => c.IsNormalizer).ToList();
            if (normalizer.Count != 1) return 0;
            var value = normalizer[0].BasicParameterSet.GetAmountParam(SelectedBuildupUnit).Amount;
            if (value.HasValue)
            {
                return 1 / value.Value;
            }
            return 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
