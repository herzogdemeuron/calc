using Calc.Core;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    public class BuilderViewModel: INotifyPropertyChanged
    {
        public DirectusStore Store { get; set; }
        public BuildupCreationViewModel BuildupCreationVM { get; set; }
        public LoadingViewModel LoadingVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }

        public BuilderViewModel(DirectusStore store, IBuildupComponentCreator builupComponentCreator)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel();
            LoadingVM = new LoadingViewModel(store);
            BuildupCreationVM = new BuildupCreationViewModel(Store, builupComponentCreator);
        }

        public async Task HandleWindowLoadedAsync()
        {
            await LoadingVM.HandleBuilderLoading();
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

        public void HandleComponentSelectionChanged(ICalcComponent selectedCompo)
        {
            BuildupCreationVM.HandleComponentSelectionChanged(selectedCompo);
            //NodeTreeVM.HandleNodeItemSelectionChanged(selectedBranch);
        }

        public void HandleSideClicked()
        {
            //NodeTreeVM.DeselectNodes();
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
