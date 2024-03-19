using Calc.Core;
using Calc.Core.Calculations;
using Calc.Core.Helpers;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Models;
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
        private List<Material> CurrentMaterials { get => store.CurrentMaterials; }

        private readonly IBuildupComponentCreator buildupComponentCreator;
        public bool MaterialSelectionEnabled { get => SelectedComponent is LayerComponent; }

        public List<CalculationComponent> AllCalculationComponents
        {
            get
            {
                var components = new List<CalculationComponent>();
                foreach (var component in BuildupComponents)
                {
                    components.AddRange(component.CalculationComponents.Where(c => c.Amount.HasValue));
                }
                return components;
            }
        }
        public List<CalculationComponent> AllErrorCalculationComponents
        {
            get
            {
                var components = new List<CalculationComponent>();
                foreach (var component in BuildupComponents)
                {
                    components.AddRange(component.CalculationComponents.Where(c => c.HasError));
                }
                return components;
            }
        }

        private Dictionary<LayerComponent, LayerModel> layerModels = new Dictionary<LayerComponent, LayerModel>();
        public LayerModel CurrentLayerModel
        {
            get
            {
                if (layerModels == null) return null;
                if (SelectedComponent is LayerComponent layerComponent)
                {
                  return layerModels.TryGetValue(layerComponent, out var layerModel) ? layerModel : null;
                }
                return null;
            }
        }

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

        public LcaStandard SelectedStandard
        {
            get => store.StandardSelected;
            set
            {
                if (store.StandardSelected != value)
                {
                    store.StandardSelected = value;
                    UpdateLayerModels();
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
            OnPropertyChanged(nameof(SelectedStandard));
            OnPropertyChanged(nameof(BuildupUnitsAll));
            OnPropertyChanged(nameof(BuildupGroupsAll));
        }
        public void HandleSelectingElements()
        {
            BuildupComponents = buildupComponentCreator.CreateBuildupComponentsFromSelection();
            UpdateLayerModels();
            UpdateCalculationComponents();
        }

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            SelectedComponent = selectedCompo;
            NotifyAmountChanged();
            NotifyMaterialsChanged();
        }

        public void HandleReduceMaterial()
        {
            if(CurrentLayerModel != null)
            {
                CurrentLayerModel.RemoveSubMaterial();
            }
        }

        private void UpdateLayerModels()
        {
            layerModels?.Clear();
            foreach (var component in BuildupComponents.Where(c => c.HasLayers))
            {
                foreach (var layer in component.LayerComponents)
                {
                    layerModels.Add(layer, new LayerModel(layer, CurrentMaterials, MaterialFunctionsAll));
                }
            }
            OnPropertyChanged(nameof(CurrentLayerModel));
            CurrentLayerModel?.NotifyPropertiesChange();
        }

        public void UpdateCalculationComponents()
        {
            var quantityRatio = GetQuantityRatio();

            foreach (var component in BuildupComponents)
            {
                component.UpdateCalculationComponents(quantityRatio);
            }
            OnPropertyChanged(nameof(AllCalculationComponents));
            OnPropertyChanged(nameof(AllErrorCalculationComponents));
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


        private void NotifyMaterialsChanged()
        {
            OnPropertyChanged(nameof(CurrentLayerModel));
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
