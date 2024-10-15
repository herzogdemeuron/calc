using Calc.Core;
using Calc.Core.Calculation;
using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Materials;
using Calc.Core.Objects.Standards;
using Calc.Core.Snapshots;
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
    /// <summary>
    /// Used by the calc builder.
    /// The major VM handling the assembly creation logic.
    /// </summary>
    public class AssemblyCreationViewModel : INotifyPropertyChanged
    {
        private readonly CalcStore store;
        private readonly IElementSourceHandler elementSourceHandler;
        private readonly IImageSnapshotCreator imageSnapshotCreator;
        private readonly IElementSender elementSender;
        private Dictionary<string, string> DynamicProperties = new Dictionary<string, string>();
        public List<LcaStandard> StandardsAll { get => store.StandardsAll; }
        public List<Unit> AssemblyUnitsAll { get => store.UnitsAll; }
        public List<MaterialFunction> MaterialFunctionsAll { get => store.MaterialFunctionsAll; }
        public List<AssemblyGroup> AssemblyGroupsAll { get => store.AssemblyGroupsAll; }
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
        private ObservableCollection<AssemblyComponent> assemblyComponents = new ObservableCollection<AssemblyComponent>();
        public ObservableCollection<AssemblyComponent> AssemblyComponents
        {
            get => assemblyComponents;
            set
            {
                if (assemblyComponents == value) return;
                assemblyComponents = value;
                OnPropertyChanged(nameof(AssemblyComponents));
            }
        }
        public List<CalculationComponent> CurrentCalculationComponents
        {
            get
            {
                if (AssemblyComponents.Count == 0) return null;
                var calcs = new List<CalculationComponent>();
                foreach (var component in AssemblyComponents)
                {
                    calcs.AddRange(component.GetCalculationComponents());
                }
                CalculationComponent.UpdatePosition(calcs);
                return calcs;
            }
        }
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
        private string newAssemblyName;
        public string NewAssemblyName
        {
            get => newAssemblyName;
            set
            {
                if (newAssemblyName == value) return;
                newAssemblyName = value;
                CheckSaveOrUpdate();
                OnPropertyChanged(nameof(NewAssemblyName));
                OnPropertyChanged(nameof(CanSave));
            }
        }
        private string newAssemblyCode;
        public string NewAssemblyCode
        {
            get => newAssemblyCode;
            set
            {
                if (newAssemblyCode == value) return;
                newAssemblyCode = value;
                CheckSaveOrUpdate();
                OnPropertyChanged(nameof(NewAssemblyCode));
                OnPropertyChanged(nameof(CanSave));
            }
        }
        private string newAssemblyDescription;
        public string NewAssemblyDescription
        {
            get => newAssemblyDescription;
            set
            {
                if (newAssemblyDescription == value) return;
                newAssemblyDescription = value;
                OnPropertyChanged(nameof(NewAssemblyDescription));
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
        /// A set of standards from the materials in the CurrentCalculationComponents
        /// </summary>
        private List<LcaStandard> Standards
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
        private Unit? selectedAssemblyUnit;
        public Unit? SelectedAssemblyUnit // todo: put this to record
        {
            get => selectedAssemblyUnit;
            set
            {
                selectedAssemblyUnit = value;
                OnPropertyChanged(nameof(SelectedAssemblyUnit));
                UpdateAmounts();
                UpdateAssemblyComponentError();
                UpdateCalculationComponents();
                OnPropertyChanged(nameof(MainWarning));
            }
        }
        private AssemblyGroup selectedAssemblyGroup;
        public AssemblyGroup SelectedAssemblyGroup
        {
            get => selectedAssemblyGroup;
            set
            {
                if (selectedAssemblyGroup == value) return;
                selectedAssemblyGroup = value;
                OnPropertyChanged(nameof(SelectedAssemblyGroup));
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
        public double? AssemblyGwp { get => CurrentCalculationComponents?.Sum(c => c.Gwp); }
        public double? AssemblyGe { get => CurrentCalculationComponents?.Sum(c => c.Ge); }
        private string emptySelectionText = "- select elements to start -";
        public string EmptySelectionText
        {
            get => emptySelectionText;
            set
            {
                emptySelectionText = value;
                OnPropertyChanged(nameof(EmptySelectionText));
            }
        }
        public string EmptyCalculationText
        {
            get
            {
                if (CurrentCalculationComponents?.Count > 0)
                {
                    return string.Empty;
                }
                return "- no valid calculation -";

            }
        }

        public AssemblyCreationViewModel(CalcStore store, IElementSourceHandler elemSrcHandler, IImageSnapshotCreator imgCreator, IElementSender elemSender)
        {
            this.store = store;
            elementSourceHandler = elemSrcHandler;
            imageSnapshotCreator = imgCreator;
            elementSender = elemSender;
        }

        /// <summary>
        /// Initialize the some properties.
        /// </summary>
        internal void HandleLoaded()
        {
            OnPropertyChanged(nameof(AssemblyUnitsAll));
            OnPropertyChanged(nameof(AssemblyGroupsAll));
            UpadteMainWarning();
        }

        /// <summary>
        /// Clears selected component.
        /// </summary>
        internal void HandleDeselect()
        {
            SelectedComponent = null;            
            OnPropertyChanged(nameof(CurrentColor));
        }

        /// <summary>
        /// Sets Normalizer and unit.
        /// </summary>
        /// <param name="newAssemblyUnit"></param>
        internal void HandleAmountClicked(Unit? newAssemblyUnit)
        {
            if(newAssemblyUnit == null) return;
            foreach (var component in AssemblyComponents)
            {
                component.IsNormalizer = false;
                component.HasParamError = false;
            }
            if (SelectedComponent is AssemblyComponent bc)
            {
                bc.IsNormalizer = true;
            }
            SelectedAssemblyUnit = newAssemblyUnit;
            UpadteMainWarning();
        }

        internal void HandleCaptureClicked()
        {
            string filename = NewAssemblyName ?? "CalcAssembly";
            currentImagePath = imageSnapshotCreator.CreateImageSnapshot(filename);
            CurrentImage = new BitmapImage(new Uri(currentImagePath));
        }

        /// <summary>
        /// Sets the capture text, for mouse enter and leave area.
        /// </summary>
        /// <param name="isEnter"></param>
        internal void HandleCaptureMouseOver(bool isEnter)
        {
            if(CurrentImage == null) return;
            CaptureText = isEnter ? "↻" : "";
        }

        private void UpadteMainWarning()
        {
            if (AssemblyComponents.Count == 0)
            {
                MainWarning = "Select elements (with Revit materials applied) to start.";
                return;
            }
            var hasNormalizer = AssemblyComponents.Where(c => c.IsNormalizer).ToList().Count == 1;
            MainWarning = hasNormalizer ? "" : "Choose a type 🡫 and set one unit as normalizer 🡩 ";
        }

        private void UpadteEmptyText()
        {
            if (AssemblyComponents.Count == 0)
            {
                EmptySelectionText = "- select elements to start -";
                return;
            }
            EmptySelectionText = string.Empty;
        }

        private void UpdateAssemblyComponentError()
        {
            if (SelectedComponent is AssemblyComponent bc)
            {
                bc.UpdateParamError((Unit)SelectedAssemblyUnit);
            }
        }

        /// <summary>
        /// Updates the basic unit amounts of the current selection.
        /// </summary>
        private void UpdateAmounts()
        {
            var materialUnit = CurrentLayerMaterialModel?.MainMaterial?.MaterialUnit;
            AmountsModel.UpdateAmounts(SelectedComponent, SelectedAssemblyUnit, materialUnit);
        }

        /// <summary>
        /// Selects elements from revit, gets the assembly components,
        /// restores assembly record to both the assembly components and the current view model.
        /// </summary>
        internal void HandleSelectingElements()
        {
            try
            {
                // get the selection result from revit, including metadata and recorded materials
                var result = elementSourceHandler.SelectElements(store.CustomParamSettingsAll);
                var record = elementSourceHandler.GetAssemblyRecord();
                if (record != null) result.ApplyAssemblyRecord(record, store);

                AssemblyComponents = new ObservableCollection<AssemblyComponent>(result.AssemblyComponents);
                DynamicProperties = result.Parameters;
                if (result.AssemblyCode != null)  NewAssemblyCode = result.AssemblyCode;
                if (result.AssemblyName != null) NewAssemblyName = result.AssemblyName;
                if (result.AssemblyGroup != null) SelectedAssemblyGroup = result.AssemblyGroup;
                if (result.Description != null) NewAssemblyDescription = result.Description;
                SelectedAssemblyUnit = result.AssemblyUnit;

                UpdateLayerMaterialModels();
                UpdateLayerColors();
                UpdateCalculationComponents();
                UpadteMainWarning();
                UpadteEmptyText();

                CurrentImage = null;
                CaptureText = "📷";
                saveMessage = "";
            }
            catch (Exception ex)
            {
                MainWarning = ex.Message;
            }

        }

        /// <summary>
        /// Sets the selcted component.
        /// Called while component selection of treeview changed.
        /// </summary>
        internal void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            SelectedComponent = selectedCompo;
            // set the main material tab to active
            CurrentLayerMaterialModel.ResetActiveMaterial();
            OnPropertyChanged(nameof(CurrentColor));
        }

        /// <summary>
        /// Sets the material to the layer material model.
        /// </summary>
        internal void HandleSetMaterial(bool setMain, Material material)
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

        /// <summary>
        /// Removes one material from the layer material model.
        /// </summary>
        internal void HandleReduceMaterial()
        {
            CurrentLayerMaterialModel.RemoveMaterial();
        }

        /// <summary>
        /// Reloads all layer material models.
        /// Called when the standard changes or selected revit elements changed.
        /// </summary>
        private void UpdateLayerMaterialModels()
        {
            layerMaterialModels?.Clear();
            foreach (var component in AssemblyComponents.Where(c => c.HasLayers))
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

        /// <summary>
        /// The properties change from layer material models should invoke this function to update ui.
        /// </summary>
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
        /// Sets the same material setting to models, according to the same target material.
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
            ItemPainter.ColorLayersByMaterial(AssemblyComponents);
            OnPropertyChanged(nameof(CurrentColor));
        }

        internal void UpdateCalculationComponents()
        {
            var normalizeRatio = GetNormalizeRatio();
            foreach (var component in AssemblyComponents)
            {
                component.UpdateCalculationComponents(normalizeRatio);
            }
            OnPropertyChanged(nameof(StandardsString));
            OnPropertyChanged(nameof(CurrentCalculationComponents));
            OnPropertyChanged(nameof(AssemblyGwp));
            OnPropertyChanged(nameof(AssemblyGe));
            OnPropertyChanged(nameof(CanSave));
            OnPropertyChanged(nameof(EmptyCalculationText));
            SaveMessage = "";
        }

        /// <summary>
        /// Gets the normalize ratio from the normalizer.
        /// </summary>
        private double GetNormalizeRatio()
        {
            var normalizer = AssemblyComponents.Where(c => c.IsNormalizer).ToList();
            if (normalizer.Count != 1) return 0;
            if (SelectedAssemblyUnit == null) return 0;
            var value = normalizer[0].BasicParameterSet.GetAmountParam((Unit)SelectedAssemblyUnit).Amount;
            if (value.HasValue)
            {
                return 1 / value.Value;
            }
            return 0;
        }

        /// <summary>
        /// Creates the assembly class before saving.
        /// Taking the image snapshot and the speckle properties.
        /// </summary>
        private Assembly CreateAssembly(string imageUuid,string speckleProjectId, string speckleModelId)
        {
            var assembly = new Assembly
            {
                Name = NewAssemblyName,
                Code = NewAssemblyCode,
                Description = NewAssemblyDescription,
                StandardItems = Standards.Select(s => new StandardItem { Standard = s }).ToList(),
                Group = SelectedAssemblyGroup,
                AssemblyUnit = (Unit)SelectedAssemblyUnit,
                CalculationComponents = CurrentCalculationComponents,
                AssemblyGwp = AssemblyGwp,
                AssemblyGe = AssemblyGe,
            };
            SnapshotMaker.Snap(assembly);
            if (!string.IsNullOrEmpty(imageUuid))
            {
                assembly.AssemblyImage = new AssemblyImage() { Id = imageUuid };
            }
            if (!string.IsNullOrEmpty(speckleModelId) && !string.IsNullOrEmpty(speckleProjectId))
            {
                assembly.SpeckleProjectId = speckleProjectId;
                assembly.SpeckleModelId = speckleModelId;
            }
            return assembly;
        }
        
        /// <summary>
        /// Uploads the image snapshot and speckle model, 
        /// creates the assembly and saves it.
        /// Lastly takes care of the assembly record.
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> HandleSaveAssembly()
        {
            IsNotSaving = false;
            SavingVisibility = Visibility.Visible;
            SaveMessage = "";
            try
            {
                string imageUuid = await store.UploadImageAsync(currentImagePath, NewAssemblyName); // todo: make this more robust
                var speckleModelId = await SendElementsToSpeckle();
                var speckleProjectId = store.Config.SpeckleBuilderProjectId;
                var assembly = CreateAssembly(imageUuid, speckleProjectId, speckleModelId);
                if (updateId != null)
                {
                    assembly.Id = updateId.Value;
                    await store.UpdateSingleAssembly(assembly);
                    SaveMessage = "Assembly updated.";
                }
                else
                {
                    await store.SaveSingleAssembly(assembly);
                    SaveMessage = "New Assembly saved.";
                    CheckSaveOrUpdate();
                }
                // store the assembly record
                elementSourceHandler.SaveAssemblyRecord
                    (
                        newAssemblyCode, 
                        newAssemblyName, 
                        SelectedAssemblyUnit.Value,
                        SelectedAssemblyGroup, 
                        newAssemblyDescription, 
                        AssemblyComponents.ToList() 
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
        /// Sends elements to speckle, gets the model id.
        /// </summary>
        private async Task<string> SendElementsToSpeckle()
        {
            if (!elementSender.IsValid) return null;
            var elementIds = AssemblyComponents.SelectMany(c => c.ElementIds).ToList();
            var assemblyData = new AssemblyData()
            {
                Name = NewAssemblyName,
                Code = NewAssemblyCode,
                ElementIds = elementIds,
                Description = NewAssemblyDescription,
                Group = SelectedAssemblyGroup.Name,
                Properties = DynamicProperties
            };
            return await elementSender.SendAssembly(assemblyData);
        }

        private bool CheckCanSave()
        {
            return !string.IsNullOrEmpty(NewAssemblyCode) && !string.IsNullOrEmpty(NewAssemblyName) && SelectedAssemblyGroup != null && CurrentCalculationComponents.Count > 0 && IsNotSaving;
        }

        private void CheckSaveOrUpdate()
        {
            var allAssemblies = store.AssembliesAll;
            // find the id with the same code
            var existingAssembly = allAssemblies.Find(b => b.Code != null && b.Code == NewAssemblyCode);
            if(existingAssembly != null)
            {
                updateId = existingAssembly.Id;
                SaveOrUpdate = false;
            }
            else
            {
                updateId = null;
                SaveOrUpdate = true;
            }
        }
        internal void MoveAssemblyComponent(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex) return;
            AssemblyComponents.Move(oldIndex, newIndex);
            UpdateCalculationComponents();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
