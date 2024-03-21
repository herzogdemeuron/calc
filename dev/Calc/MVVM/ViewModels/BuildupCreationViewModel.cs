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
using System.Threading.Tasks;
using System.Windows;

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
                CalculationComponent.UpdatePosition(components);
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

        private Dictionary<LayerComponent,  LayerModel> layerModels = new Dictionary<LayerComponent, LayerModel>();
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

        private string newBuildupName;
        public string NewBuildupName
        {
            get => newBuildupName;
            set
            {
                if (newBuildupName == value) return;
                newBuildupName = value;
                OnPropertyChanged(nameof(NewBuildupName));
            }
        }

        private string newBuildupDescription;
        public string NewBuildupDescription
        {
            get => newBuildupDescription;
            set
            {
                if (newBuildupDescription == value) return;
                newBuildupDescription = value;
                OnPropertyChanged(nameof(NewBuildupDescription));
            }
        }

        private ICalcComponent selectedComponent;
        public ICalcComponent SelectedComponent
        {
            get => selectedComponent;
            set
            {
                if (selectedComponent == value) return;
                selectedComponent = value;
                OnPropertyChanged(nameof(SelectedComponent));
            }
        }

        public LcaStandard SelectedStandard
        {
            get => store.StandardSelected;
            set
            {
                if (store.StandardSelected == value) return;
                store.StandardSelected = value;
                UpdateLayerModels();
                OnPropertyChanged(nameof(SelectedStandard));
            }
        }

        private Unit? selectedBuildupUnit;
        public Unit? SelectedBuildupUnit
        {
            get => selectedBuildupUnit;
            set
            {
                if (selectedBuildupUnit == value) return;
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
                if (selectedBuildupGroup == value) return;
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
                if (buildupComponents == value) return;
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
            SetNormalizer(); // temporary only for testing
            UpdateCalculationComponents();
        }

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            SelectedComponent = selectedCompo;
            NotifyAmountChanged();
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
                    var layerModel = new LayerModel(layer, CurrentMaterials, MaterialFunctionsAll);
                    layerModel.MaterialPropertyChanged += HandleMaterialChanged;
                    layerModels.Add(layer, layerModel);
                }
            }
            OnPropertyChanged(nameof(CurrentLayerModel));
            CurrentLayerModel?.NotifyPropertiesChange();
        }

        // on material chaned
        private void HandleMaterialChanged(object sender, EventArgs e)
        {
            UpdateCalculationComponents();
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

        private void SetNormalizer() // temporary only for testing
        {
            BuildupComponents.First().IsNormalizer = true;
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

        private Buildup CreateBuildup()
        {
            var buildup = new Buildup
            {
                Name = NewBuildupName,
                Description = NewBuildupDescription,
                Standard = SelectedStandard,
                Group = SelectedBuildupGroup,
                BuildupUnit = (Unit)SelectedBuildupUnit,
                CalculationComponents = AllCalculationComponents
            };
            return buildup;
        }

        public async Task<bool> HandleSaveBuildup()
        {
            var buildup = CreateBuildup();
            bool response = await store.SaveSingleBuildup(buildup);
            return response;

        }
        private void NotifyAmountChanged()
        {
            OnPropertyChanged(nameof(CountString));
            OnPropertyChanged(nameof(LengthString));
            OnPropertyChanged(nameof(AreaString));
            OnPropertyChanged(nameof(VolumeString));
            OnPropertyChanged(nameof(CurrentLayerModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
