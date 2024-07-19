using Calc.Core;
using Calc.Core.Calculation;
using Calc.Core.Color;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Standards;
using Calc.Core.Snapshots;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
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
        private readonly CalcStore store;
        private readonly IElementSourceHandler elementSourceHandler;
        private readonly IImageSnapshotCreator imageSnapshotCreator;
        private readonly IElementSender elementSender;

        private Dictionary<string, string> DynamicProperties = new Dictionary<string, string>();
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

        public List<CalculationComponent> CurrentCalculationComponents
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
                newBuildupName = value.Trim();
                CheckSaveOrUpdate();
                OnPropertyChanged(nameof(NewBuildupName));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        private string newBuildupCode;
        public string NewBuildupCode
        {
            get => newBuildupCode;
            set
            {
                if (newBuildupCode == value) return;
                newBuildupCode = value.Trim();
                CheckSaveOrUpdate();
                OnPropertyChanged(nameof(NewBuildupCode));
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

        /// <summary>
        /// the set of standard from the materials in the AllCalculationComponents
        /// </summary>
        public List<LcaStandard> Standards
        {
            get => CurrentCalculationComponents?
                .Where(c => c.Material != null)
                .Select(c => c.Material.Standard)
                .Distinct().ToList();
        }

        public string StandardsString
        {
            get => Standards == null ? "" : string.Join(", ", Standards.Select(s => s.Name));
        }

        private Unit? selectedBuildupUnit;
        public Unit? SelectedBuildupUnit // todo: put this to record
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

        private int? updateId;
        private bool saveOrUpdate = true;
        public bool SaveOrUpdate
        {
            get => saveOrUpdate;
            set
            {
                if (saveOrUpdate == value) return;
                saveOrUpdate = value;
                OnPropertyChanged(nameof(SaveOrUpdate));
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

        public double? BuildupGwp { get => CurrentCalculationComponents?.Sum(c => c.Gwp); }
        public double? BuildupGe { get => CurrentCalculationComponents?.Sum(c => c.Ge); }


        public BuildupCreationViewModel(CalcStore store, IElementSourceHandler elemSrcHandler, IImageSnapshotCreator imgCreator, IElementSender elemSender)
        {
            this.store = store;
            elementSourceHandler = elemSrcHandler;
            imageSnapshotCreator = imgCreator;
            elementSender = elemSender;
        }

        public void HandleLoaded()
        {
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
        /// selecting elements from revit, get the buildup components 
        /// and try to restore buildup record to both the buildup components and the current view model
        /// </summary>
        public void HandleSelectingElements()
        {
            // get the selection result from revit, including metadata and recorded materials
            var result = elementSourceHandler.SelectElements(store.CustomParamSettingsAll);
            var record = elementSourceHandler.GetBuildupRecord();
            result.ApplyBuildupRecord(record, store);

            BuildupComponents = new ObservableCollection<BuildupComponent>(result.BuildupComponents);
            DynamicProperties = result.Parameters;
            if (result.BuildupCode != null)  NewBuildupCode = result.BuildupCode;
            if (result.BuildupName != null) NewBuildupName = result.BuildupName;
            if (result.BuildupGroup != null) SelectedBuildupGroup = result.BuildupGroup;
            if (result.Description != null) NewBuildupDescription = result.Description;
            SelectedBuildupUnit = result.BuildupUnit;

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
                    var layerMaterialModel = new LayerMaterialModel(layer, store.MaterialsAll, MaterialFunctionsAll);
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
            var normalizeRatio = GetNormalizeRatio();

            foreach (var component in BuildupComponents)
            {
                component.UpdateCalculationComponents(normalizeRatio);
            }
            OnPropertyChanged(nameof(StandardsString));
            OnPropertyChanged(nameof(CurrentCalculationComponents));
            OnPropertyChanged(nameof(BuildupGwp));
            OnPropertyChanged(nameof(BuildupGe));
            OnPropertyChanged(nameof(CanSave));
            SaveMessage = "";
        }


        /// <summary>
        /// get the normalize ratio from the normalizer
        /// </summary>
        private double GetNormalizeRatio()
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

        private Buildup CreateBuildup(string imageUuid,string speckleProjectId, string speckleModelId)
        {
            var buildup = new Buildup
            {
                Name = NewBuildupName,
                Code = NewBuildupCode,
                Description = NewBuildupDescription,
                StandardItems = Standards.Select(s => new StandardItem { Standard = s }).ToList(),
                Group = SelectedBuildupGroup,
                BuildupUnit = (Unit)SelectedBuildupUnit,
                CalculationComponents = CurrentCalculationComponents,
                BuildupGwp = BuildupGwp,
                BuildupGe = BuildupGe,
            };
            SnapshotMaker.Snap(buildup);

            if (!string.IsNullOrEmpty(imageUuid))
            {
                buildup.BuildupImage = new BuildupImage() { Id = imageUuid };
            }

            if (!string.IsNullOrEmpty(speckleModelId) && !string.IsNullOrEmpty(speckleProjectId))
            {
                buildup.SpeckleProjectId = speckleProjectId;
                buildup.SpeckleModelId = speckleModelId;
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
                var speckleModelId = await SendElementsToSpeckle();
                var speckleProjectId = store.Config.SpeckleBuilderProjectId;
                var buildup = CreateBuildup(imageUuid, speckleProjectId, speckleModelId);

                if (updateId != null)
                {
                    buildup.Id = updateId.Value;
                    await store.UpdateSingleBuildup(buildup);
                    SaveMessage = "Assembly updated.";
                }
                else
                {
                    await store.SaveSingleBuildup(buildup);
                    SaveMessage = "New Assembly saved.";
                    CheckSaveOrUpdate();
                }


                // store the buildup record
                elementSourceHandler.SaveBuildupRecord
                    (
                        newBuildupCode, 
                        newBuildupName, 
                        SelectedBuildupUnit.Value,
                        SelectedBuildupGroup, 
                        newBuildupDescription, 
                        BuildupComponents.ToList() 
                    );

            }
            catch (Exception ex)
            {
                SaveMessage = $"{ex.Message}";
                SaveMessageColor = Brushes.Crimson;
                SavingVisibility = Visibility.Collapsed;
                IsNotSaving = true;
                return false;
            }

            SaveMessageColor = Brushes.ForestGreen;
            SavingVisibility = Visibility.Collapsed;
            IsNotSaving = true;


            return true;

        }

        /// <summary>
        /// send elements to speckle, get the branch id 
        /// </summary>
        private async Task<string> SendElementsToSpeckle()
        {
            var elementIds = BuildupComponents.SelectMany(c => c.ElementIds).ToList();
            return await elementSender.SendToSpeckle(elementIds, NewBuildupCode, NewBuildupDescription, DynamicProperties);
        }

        private bool CheckCanSave()
        {
            return !string.IsNullOrEmpty(NewBuildupCode) && !string.IsNullOrEmpty(NewBuildupName) && SelectedBuildupGroup != null && CurrentCalculationComponents.Count > 0 && IsNotSaving;
        }

        private void CheckSaveOrUpdate()
        {
            var allBuildups = store.BuildupsAll;
            // find the id with the same code
            var existingBuildup = allBuildups.Find(b => b.Code != null && b.Code == NewBuildupCode);
            if(existingBuildup != null)
            {
                updateId = existingBuildup.Id;
                SaveOrUpdate = false;
            }
            else
            {
                updateId = null;
                SaveOrUpdate = true;
            }
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
