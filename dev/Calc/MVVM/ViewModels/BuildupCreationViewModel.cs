using Calc.Core;
using Calc.Core.Calculations;
using Calc.Core.Color;
using Calc.Core.Helpers;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Calc.MVVM.ViewModels
{

    public class BuildupCreationViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        private List<Material> CurrentMaterials { get => store.CurrentMaterials; }
        private readonly IBuildupComponentCreator buildupComponentCreator;
        private readonly IImageSnapshotCreator imageSnapshotCreator;

        public List<LcaStandard> StandardsAll { get => store.StandardsAll; }
        public List<Unit> BuildupUnitsAll { get => store.UnitsAll; }
        public List<MaterialFunction> MaterialFunctionsAll { get => store.MaterialFunctionsAll; }
        public List<BuildupGroup> BuildupGroupsAll { get => store.BuildupGroupsAll; }

        public BasicAmountsModel AmountsModel { get; set; } = new BasicAmountsModel();
        public bool MaterialSelectionEnabled { get => SelectedComponent is LayerComponent; }
        public HslColor CurrentColor { get => SelectedComponent?.HslColor?? ItemPainter.DefaultColor; }
        private string mainWarning;
        public string MainWarning
        {
            get => mainWarning;
            set
            {
                if (mainWarning == value) return;
                mainWarning = value;
                OnPropertyChanged(nameof(MainWarning));
            }
        }


        private ObservableCollection<BuildupComponent> buildupComponents = new ObservableCollection<BuildupComponent>();
        public ObservableCollection<BuildupComponent> BuildupComponents
        {
            get => buildupComponents;
            set
            {
                if (buildupComponents == value) return;
                buildupComponents = value;
                OnPropertyChanged(nameof(BuildupComponents));
            }
        }

        public List<CalculationComponent> AllCalculationComponents
        {
            get
            {
                var calcs = new List<CalculationComponent>();
                foreach (var component in BuildupComponents)
                {
                    calcs.AddRange(component.GetCalculationComponents());
                }
                CalculationComponent.UpdatePosition(calcs);
                return calcs;
            }
        }

        /// <summary>
        /// Each layer component has a layer material model, which defines the material settings for the layer
        /// </summary>
        private Dictionary<LayerComponent,  LayerMaterialModel> layerMaterialModels = new Dictionary<LayerComponent, LayerMaterialModel>();
        private LayerMaterialModel InvalidLayerMaterialModel { get => new LayerMaterialModel(); }
        public LayerMaterialModel CurrentLayerMaterialModel
        {
            get
            {
                if (layerMaterialModels == null) return null;
                if (SelectedComponent is LayerComponent layerComponent)
                {
                  return layerMaterialModels.TryGetValue(layerComponent, out var layerMaterialModel) ? layerMaterialModel : InvalidLayerMaterialModel;
                }
                return InvalidLayerMaterialModel;
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
                OnPropertyChanged(nameof(CanSave));
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
                OnPropertyChanged(nameof(CurrentLayerMaterialModel));
                UpdateAmounts();
            }
        }

        public LcaStandard SelectedStandard
        {
            get => store.StandardSelected;
            set
            {
                if (store.StandardSelected == value) return;
                store.StandardSelected = value;
                UpdateLayerMaterialModels();
                OnPropertyChanged(nameof(SelectedStandard));
                UpdateLayerColors();
                UpdateCalculationComponents();
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
                UpdateAmounts();
                UpdateBuildupComponentError();
                UpdateCalculationComponents();
                OnPropertyChanged(nameof(MainWarning));
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
                OnPropertyChanged(nameof(CanSave));
            }
        }

        private string currentImagePath;
        private BitmapImage currentImage;
        public BitmapImage CurrentImage
        {
            get => currentImage;
            set
            {
                if (currentImage == value) return;
                currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        private string captureText = "📷";
        public string CaptureText
        {
            get => captureText;
            set
            {
                if (captureText == value) return;
                captureText = value;
                OnPropertyChanged(nameof(CaptureText));
            }
        }

        private string saveMessage;
        public string SaveMessage
        {
            get => saveMessage;
            set
            {
                if (saveMessage == value) return;
                saveMessage = value;
                OnPropertyChanged(nameof(SaveMessage));
            }
        }

        private SolidColorBrush saveMessageColor;
        public SolidColorBrush SaveMessageColor
        {
            get => saveMessageColor;
            set
            {
                if (saveMessageColor == value) return;
                saveMessageColor = value;
                OnPropertyChanged(nameof(SaveMessageColor));
            }
        }

        public bool CanSave { get => CheckCanSave(); }

        private bool isNotSaving = true;
        public bool IsNotSaving
        {
            get => isNotSaving;
            set
            {
                if (isNotSaving == value) return;
                isNotSaving = value;
                OnPropertyChanged(nameof(IsNotSaving));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        private Visibility savingVisibility = Visibility.Collapsed;
        public Visibility SavingVisibility
        {
            get => savingVisibility;
            set
            {
                if (savingVisibility == value) return;
                savingVisibility = value;
                OnPropertyChanged(nameof(SavingVisibility));
            }
        }

        public double? BuildupGwp { get => AllCalculationComponents?.Where(c => c.Amount.HasValue).Sum(c => c.Gwp); }
        public double? BuildupGe { get => AllCalculationComponents?.Where(c => c.Amount.HasValue).Sum(c => c.Ge); }


        public BuildupCreationViewModel(DirectusStore store, IBuildupComponentCreator bcCreator, IImageSnapshotCreator imgCreator)
        {
            this.store = store;
            buildupComponentCreator = bcCreator;
            imageSnapshotCreator = imgCreator;
        }

        public void HandleLoaded()
        {
            OnPropertyChanged(nameof(StandardsAll));
            OnPropertyChanged(nameof(SelectedStandard));
            OnPropertyChanged(nameof(BuildupUnitsAll));
            OnPropertyChanged(nameof(BuildupGroupsAll));
            UpadteMainWarning();
        }

        public void HandleDeselect()
        {
            SelectedComponent = null;
            MediatorToView.Broadcast("ViewDeselectTreeView");
            OnPropertyChanged(nameof(CurrentColor));
        }

        public void HandleAmountClicked(Unit? newBuildupUnit)
        {
            if(newBuildupUnit == null) return;
            foreach (var component in BuildupComponents)
            {
                component.IsNormalizer = false;
                component.HasParamError = false;
            }
            if (SelectedComponent is BuildupComponent bc)
            {
                bc.IsNormalizer = true;
            }
            SelectedBuildupUnit = newBuildupUnit;
            UpadteMainWarning();
        }

        public void HandleCaptureClicked()
        {
            string filename = NewBuildupName ?? "CalcBuildup";
            currentImagePath = imageSnapshotCreator.CreateImageSnapshot(filename);
            CurrentImage = new BitmapImage(new Uri(currentImagePath));
        }

        public void HandleCaptureMouseOver(bool isEnter)
        {
            if(CurrentImage == null) return;
            CaptureText = isEnter ? "↻" : "";
        }

        private void UpadteMainWarning()
        {
            if (BuildupComponents.Count == 0)
            {
                MainWarning = "Select elements to start.";
                return;
            }
            var hasNormalizer = BuildupComponents.Where(c => c.IsNormalizer).ToList().Count == 1;
            MainWarning = hasNormalizer ? "" : "Choose a type and set one amount as normalizer.";
        }

        private void UpdateBuildupComponentError()
        {
            if (SelectedComponent is BuildupComponent bc)
            {
                bc.UpdateParamError((Unit)SelectedBuildupUnit);
            }
        }

        /// <summary>
        /// updates the basic unit amounts of the current selection
        /// </summary>
        private void UpdateAmounts()
        {
            var materialUnit = CurrentLayerMaterialModel?.MainMaterial?.MaterialUnit;
            AmountsModel.UpdateAmounts(SelectedComponent, SelectedBuildupUnit, materialUnit);
        }

        /// <summary>
        /// selecting elements from revit
        /// </summary>
        public void HandleSelectingElements()
        {
            var components = buildupComponentCreator.CreateBuildupComponentsFromSelection(store.CustomParamSettingsAll);
            BuildupComponents = new ObservableCollection<BuildupComponent>(components);

            UpdateLayerMaterialModels();
            UpdateLayerColors();
            UpdateCalculationComponents();
            UpadteMainWarning();

            CurrentImage = null;
            CaptureText = "📷";
            saveMessage = "";
        }

        /// <summary>
        /// component selection from treeview changed
        /// </summary>
        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            SelectedComponent = selectedCompo;
            // set the main material tab to active
            CurrentLayerMaterialModel.ResetActiveMaterial();
            OnPropertyChanged(nameof(CurrentColor));
        }

        public void HandleSetMaterial(bool setMain, Material material)
        {
            if (setMain)
            {
                if (CurrentLayerMaterialModel.MainMaterial == material) return;
                CurrentLayerMaterialModel.MainMaterial = material;
            }
            else
            {
                if (CurrentLayerMaterialModel.SubMaterial == material) return;
                CurrentLayerMaterialModel.SubMaterial = material;
            }
        }

        public void HandleReduceMaterial()
        {
            CurrentLayerMaterialModel.RemoveMaterial();
        }

        /// <summary>
        /// reload all layer material models, when the standard changes or selected revit elements changed
        /// </summary>
        private void UpdateLayerMaterialModels()
        {
            layerMaterialModels?.Clear();
            foreach (var component in BuildupComponents.Where(c => c.HasLayers))
            {
                foreach (var layer in component.LayerComponents)
                {
                    var layerMaterialModel = new LayerMaterialModel(layer, CurrentMaterials, MaterialFunctionsAll);
                    layerMaterialModel.MaterialPropertyChanged += HandleMaterialChanged;
                    layerMaterialModels.Add(layer, layerMaterialModel);
                }
            }
            OnPropertyChanged(nameof(CurrentLayerMaterialModel));
            CurrentLayerMaterialModel.NotifyPropertiesChange();
        }

        // the properties change from layer material models should invoke this function to update ui
        private void HandleMaterialChanged(object sender, EventArgs e)
        {
            if (sender is LayerMaterialModel changedModel)
            {
                UpdateMaterialModelSettings(changedModel);
            }
            UpdateLayerColors();
            UpdateCalculationComponents();
            UpdateAmounts();
        }

        /// <summary>
        /// set the same material setting to models with the same target material
        /// </summary>
        private void UpdateMaterialModelSettings(LayerMaterialModel changedModel)
        {
            foreach (var model in layerMaterialModels.Values)
            {
                if (model.CheckIdenticalTargetMaterial(changedModel))
                {
                    model.LearnMaterialSetting(changedModel);
                }
            }
        }

        private void UpdateLayerColors()
        {
            ItemPainter.ColorLayersByMaterial(BuildupComponents);
            OnPropertyChanged(nameof(CurrentColor));
        }

        public void UpdateCalculationComponents()
        {
            var quantityRatio = GetQuantityRatio();

            foreach (var component in BuildupComponents)
            {
                component.UpdateCalculationComponents(quantityRatio);
            }
            OnPropertyChanged(nameof(AllCalculationComponents));
            OnPropertyChanged(nameof(BuildupGwp));
            OnPropertyChanged(nameof(BuildupGe));
            OnPropertyChanged(nameof(CanSave));
            SaveMessage = "";
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

        private Buildup CreateBuildup(string imageUuid)
        {
            var buildup = new Buildup
            {
                Name = NewBuildupName,
                Description = NewBuildupDescription,
                Standard = SelectedStandard,
                Group = SelectedBuildupGroup,
                BuildupUnit = (Unit)SelectedBuildupUnit,
                CalculationComponents = AllCalculationComponents,
                BuildupGwp = BuildupGwp??0,
                BuildupGe = BuildupGe??0
            };

            if (!string.IsNullOrEmpty(imageUuid))
            {
                buildup.Image = imageUuid;
            }

            return buildup;
        }

        public async Task<bool> HandleSaveBuildup()
        {
            IsNotSaving = false;
            SavingVisibility = Visibility.Visible;
            SaveMessage = "";

            try
            {
                string imageUuid = await store.UploadImageAsync(currentImagePath, NewBuildupName); // todo: make this more robust
                var buildup = CreateBuildup(imageUuid);
                await store.SaveSingleBuildup(buildup);
            }
            catch (Exception ex)
            {
                SaveMessage = $"{ex.Message}";
                SaveMessageColor = Brushes.Crimson;
                SavingVisibility = Visibility.Collapsed;
                IsNotSaving = true;
                return false;
            }


            SaveMessage = "New Buildup saved.";
            SaveMessageColor = Brushes.ForestGreen;
            SavingVisibility = Visibility.Collapsed;
            IsNotSaving = true;

            return true;

        }

        private bool CheckCanSave()
        {
            return !string.IsNullOrEmpty(NewBuildupName) && SelectedBuildupGroup != null && AllCalculationComponents.Count > 0 && IsNotSaving;
        }

        public void MoveBuildupComponent(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex) return;
            BuildupComponents.Move(oldIndex, newIndex);
            UpdateCalculationComponents();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
