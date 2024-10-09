using Calc.Core;
using Calc.Core.Objects;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// The main view model for calc builder.
    /// </summary>
    public class BuilderViewModel: INotifyPropertyChanged
    {
        public CalcStore Store { get; set; }
        public AssemblyCreationViewModel AssemblyCreationVM { get; set; }
        public MaterialSelectionViewModel MaterialSelectionVM { get; set; }
        public event EventHandler DeselectTreeView;

        public BuilderViewModel(CalcStore store, IElementSourceHandler elementSourceHandler, IImageSnapshotCreator imageSnapshotCreator, IElementSender elementSender)
        {
            Store = store;
            MaterialSelectionVM = new MaterialSelectionViewModel(store);
            AssemblyCreationVM = new AssemblyCreationViewModel(Store, elementSourceHandler, imageSnapshotCreator, elementSender);
        }

        internal void HandleWindowLoaded()
        {
            AssemblyCreationVM.HandleLoaded();
        }

        internal async Task HandleSaveAssembly()
        {
            await AssemblyCreationVM.HandleSaveAssembly();
        }

        internal void HandleAmountClicked(Unit unit)
        {
            AssemblyCreationVM.HandleAmountClicked(unit);
        }

        internal void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            AssemblyCreationVM.HandleComponentSelectionChanged(selectedCompo);
        }

        internal void HandleSideClicked()
        {
            AssemblyCreationVM.HandleDeselect();
            DeselectTreeView?.Invoke(this, EventArgs.Empty);
        }

        internal void HandleSelectingMaterial(bool setMain)
        {
            var currentLayer = AssemblyCreationVM.CurrentLayerMaterialModel;
            var material = setMain? currentLayer.MainMaterial : currentLayer.SubMaterial;
            MaterialSelectionVM.PrepareMaterialSelection(material);
        }

        internal void HandleMaterialSelected(bool setMain)
        {
            var material = MaterialSelectionVM.SelectedMaterial;
            AssemblyCreationVM.HandleSetMaterial(setMain, material);
            MaterialSelectionVM.Reset();
        }

        internal void HandleReduceMaterial()
        {
            AssemblyCreationVM.HandleReduceMaterial();
        }

        internal void HandleSelect()
        {
            if (AssemblyCreationVM == null) return;
            AssemblyCreationVM.HandleSelectingElements();
        }

        internal void HandleCaptureClicked()
        {
            AssemblyCreationVM.HandleCaptureClicked();
        }

        internal void HandleCaptureMouseOver(bool isEnter)
        {
            AssemblyCreationVM.HandleCaptureMouseOver(isEnter);
        }

        internal void HandleAssemblyNameChanged(string text)
        {
            AssemblyCreationVM.NewAssemblyName = text;
        }

        internal void HandleAssemblyNameSetFinished()
        {
            AssemblyCreationVM.NewAssemblyName = AssemblyCreationVM.NewAssemblyName?.Trim();            
        }

        internal void HandleAssemblyCodeChanged(string text)
        {
            AssemblyCreationVM.NewAssemblyCode = text;
        }

        internal void HandleAssemblyCodeSetFinished()
        {
            AssemblyCreationVM.NewAssemblyCode = AssemblyCreationVM.NewAssemblyCode?.Trim();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
