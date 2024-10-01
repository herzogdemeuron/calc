using Calc.Core;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.MVVM.Helpers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    public class BuilderViewModel: INotifyPropertyChanged
    {

        public CalcStore Store { get; set; }
        public AssemblyCreationViewModel AssemblyCreationVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public MaterialSelectionViewModel MaterialSelectionVM { get; set; }
        public event EventHandler DeselectTreeView;

        public BuilderViewModel(CalcStore store, IElementSourceHandler elementSourceHandler, IImageSnapshotCreator imageSnapshotCreator, IElementSender elementSender)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel(); // is this used?
            MaterialSelectionVM = new MaterialSelectionViewModel(store);
            AssemblyCreationVM = new AssemblyCreationViewModel(Store, elementSourceHandler, imageSnapshotCreator, elementSender);
        }

        public void HandleWindowLoaded()
        {
            AssemblyCreationVM.HandleLoaded();
        }

        public async Task HandleSaveAssembly()
        {
            await AssemblyCreationVM.HandleSaveAssembly();
        }

        public void HandleAmountClicked(string unit)
        {
            Unit? newAssemblyUnit;
            switch (unit)
            {
                case "Count":
                    newAssemblyUnit = Unit.piece;
                    break;
                case "Length":
                    newAssemblyUnit = Unit.m;
                    break;
                case "Area":
                    newAssemblyUnit = Unit.m2;
                    break;
                case "Volume":
                    newAssemblyUnit = Unit.m3;
                    break;
                default:
                    return;
            }

            AssemblyCreationVM.HandleAmountClicked(newAssemblyUnit);
        }

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            AssemblyCreationVM.HandleComponentSelectionChanged(selectedCompo);
        }

        public void HandleSideClicked()
        {
            AssemblyCreationVM.HandleDeselect();
            DeselectTreeView?.Invoke(this, EventArgs.Empty);
        }

        public void HandleSelectingMaterial(bool setMain)
        {
            var currentLayer = AssemblyCreationVM.CurrentLayerMaterialModel;
            var material = setMain? currentLayer.MainMaterial : currentLayer.SubMaterial;
            MaterialSelectionVM.PrepareMaterialSelection(material);
        }

        public void HandleMaterialSelected(bool setMain)
        {
            var material = MaterialSelectionVM.SelectedMaterial;
            AssemblyCreationVM.HandleSetMaterial(setMain, material);
            MaterialSelectionVM.Reset();
        }

        public void HandleReduceMaterial()
        {
            AssemblyCreationVM.HandleReduceMaterial();
        }

        public void HandleSelect()
        {
            if (AssemblyCreationVM == null) return;
            AssemblyCreationVM.HandleSelectingElements();
        }

        public void HandleCaptureClicked()
        {
            AssemblyCreationVM.HandleCaptureClicked();
        }

        public void HandleCaptureMouseOver(bool isEnter)
        {
            AssemblyCreationVM.HandleCaptureMouseOver(isEnter);
        }

        public void HandleAssemblyNameChanged(string text)
        {
            AssemblyCreationVM.NewAssemblyName = text;
        }

        public void HandleAssemblyNameSetFinished()
        {
            AssemblyCreationVM.NewAssemblyName = AssemblyCreationVM.NewAssemblyName?.Trim();            
        }

        public void HandleAssemblyCodeChanged(string text)
        {
            AssemblyCreationVM.NewAssemblyCode = text;
        }

        public void HandleAssemblyCodeSetFinished()
        {
            AssemblyCreationVM.NewAssemblyCode = AssemblyCreationVM.NewAssemblyCode?.Trim();
        }

        public void NotifyStoreChange()
        {
            OnPropertyChanged(nameof(Store));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
