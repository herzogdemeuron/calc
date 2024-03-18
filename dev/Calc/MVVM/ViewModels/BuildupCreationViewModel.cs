using Calc.Core;
using Calc.Core.Calculations;
using Calc.Core.Helpers;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.ViewModels
{

    public class BuildupCreationViewModel : INotifyPropertyChanged
    {

        private DirectusStore store;
        public List<LcaStandard> StandardsAll { get => store.StandardsAll; }
        public List<Unit> BuildupUnitsAll { get => store.UnitsAll; }
        public List<MaterialFunction> MaterialFunctionsAll { get => store.MaterialFunctionsAll; }
        public List<BuildupGroup> BuildupGroupsAll { get => store.BuildupGroupsAll; }

        private IBuildupComponentCreator buildupComponentCreator;

        private ICalcComponent selectedComponent;
        public ICalcComponent SelectedComponent
        {
            get => selectedComponent;
            set
            {
                selectedComponent = value;
                OnPropertyChanged(nameof(SelectedComponent));
            }
        }

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

        private Unit? selectedBuildupUnit;
        public Unit? SelectedBuildupUnit
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
            set
            {
                buildupComponents = value;
                OnPropertyChanged(nameof(BuildupComponents));
            }
        }

        private List<CalculationComponent> calculationComponents = new List<CalculationComponent>();
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => calculationComponents;
            set
            {
                calculationComponents = value;
                OnPropertyChanged(nameof(CalculationComponents));
            }
        }

        public string CountString { get => GetAmountString(Unit.piece); }

        public string LengthString { get => GetAmountString(Unit.m); }

        public string AreaString { get => GetAmountString(Unit.m2); }

        public string VolumeString { get => GetAmountString(Unit.m3); }

        public BuildupCreationViewModel(DirectusStore store, IBuildupComponentCreator bcCreator)
        {
            this.store = store;
            buildupComponentCreator = bcCreator;
        }

        public void HandleLoaded()
        {
            OnPropertyChanged(nameof(StandardsAll));
            OnPropertyChanged(nameof(BuildupUnitsAll));
            OnPropertyChanged(nameof(BuildupGroupsAll));
        }
        public void HandleSelect()
        {
            BuildupComponents = buildupComponentCreator.CreateBuildupComponentsFromSelection();
            UpdateCalculationComponents();
            //OnPropertyChanged(nameof(BuildupComponents));
        }

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            SelectedComponent = selectedCompo;
            NotifyAmountChanged();
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

        private string GetAmountString(Unit unit)
        {
            return SelectedComponent?.BasicParameterSet.GetAmountParam(unit).Amount?.ToString() ?? "?";
        }

        private double GetQuantityRatio()
        {
            var normalizer = BuildupComponents.Where(c => c.IsNormalizer).ToList();
            if (normalizer.Count != 1) return 0;
            if (SelectedBuildupUnit == null) return 0;
            var value = normalizer[0].BasicParameterSet.GetAmountParam((Unit)SelectedBuildupUnit).Amount;
            if (value.HasValue)
            {
                return 1 / value.Value;
            }
            return 0;
        }


        private void NotifyAmountChanged()
        {
            OnPropertyChanged(nameof(CountString));
            OnPropertyChanged(nameof(LengthString));
            OnPropertyChanged(nameof(AreaString));
            OnPropertyChanged(nameof(VolumeString));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
