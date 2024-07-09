using Calc.Core;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Calc.MVVM.ViewModels
{
    public class BuilderViewModel: INotifyPropertyChanged
    {
        public CalcStore Store { get; set; }
        public BuildupCreationViewModel BuildupCreationVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public MaterialSelectionViewModel MaterialSelectionVM { get; set; }

        public BuilderViewModel(CalcStore store, IBuildupComponentCreator builupComponentCreator, IImageSnapshotCreator imageSnapshotCreator, IElementSender elementSender)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel();
            MaterialSelectionVM = new MaterialSelectionViewModel(store);
            BuildupCreationVM = new BuildupCreationViewModel(Store, builupComponentCreator, imageSnapshotCreator, elementSender);
        }

        public void HandleWindowLoaded()
        {
            BuildupCreationVM.HandleLoaded();
        }

        public async Task HandleSaveBuildup()
        {
            await BuildupCreationVM.HandleSaveBuildup();
        }

        public void HandleWindowClosing()
        {
            //NodeTreeVM.DeselectNodes();
        }

        public void HandleAmountClicked(string unit)
        {
            Unit? newBuildupUnit;
            switch (unit)
            {
                case "Count":
                    newBuildupUnit = Unit.piece;
                    break;
                case "Length":
                    newBuildupUnit = Unit.m;
                    break;
                case "Area":
                    newBuildupUnit = Unit.m2;
                    break;
                case "Volume":
                    newBuildupUnit = Unit.m3;
                    break;
                default:
                    return;
            }

            BuildupCreationVM.HandleAmountClicked(newBuildupUnit);
        }

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            BuildupCreationVM.HandleComponentSelectionChanged(selectedCompo);
        }

        public void HandleSideClicked()
        {
            BuildupCreationVM.HandleDeselect();
        }

        public void HandleSelectingMaterial(bool setMain)
        {
            var currentLayer = BuildupCreationVM.CurrentLayerMaterialModel;
            var material = setMain? currentLayer.MainMaterial : currentLayer.SubMaterial;
            MaterialSelectionVM.PrepareMaterialSelection(material);
        }

        public void HandleMaterialSelected(bool setMain)
        {
            var material = MaterialSelectionVM.SelectedMaterial;
            BuildupCreationVM.HandleSetMaterial(setMain, material);
            MaterialSelectionVM.Reset();
        }

        public void HandleReduceMaterial()
        {
            BuildupCreationVM.HandleReduceMaterial();
        }

        public void HandleSelect()
        {
            if (BuildupCreationVM == null) return;
            BuildupCreationVM.HandleSelectingElements();
        }

        public void HandleCaptureClicked()
        {
            BuildupCreationVM.HandleCaptureClicked();
        }

        public void HandleCaptureMouseOver(bool isEnter)
        {
            BuildupCreationVM.HandleCaptureMouseOver(isEnter);
        }

        public void HandleBuildupNameChanged(string text)
        {
            BuildupCreationVM.NewBuildupName = text;
        }

        public void HandleBuildupCodeChanged(string text)
        {
            BuildupCreationVM.NewBuildupCode = text;
        }

        public async Task HandleSendingResults(string newName)
        {
            //await SavingVM.HandleSendingResults(newName);
        }

        public void HandleMessageClose()
        {
            MediatorToView.Broadcast("HideMessageOverlay");
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
