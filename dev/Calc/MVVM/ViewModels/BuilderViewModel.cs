using Calc.Core;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    public class BuilderViewModel: INotifyPropertyChanged
    {
        public DirectusStore Store { get; set; }
        public LoadingViewModel LoadingVM { get; set; }
        public NodeTreeModel NodeTreeVM { get; set; }
        public SavingViewModel SavingVM { get; set; }
        public NewMappingViewModel NewMappingVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public CalculationViewModel CalculationVM { get; set; }

        public BuilderViewModel(DirectusStore store, IElementCreator elementCreator, IVisualizer visualizer)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel();

            LoadingVM = new LoadingViewModel(store);
            ForestVM = new ForestViewModel(store, elementCreator);
            MappingVM = new MappingViewModel(store);
            NewMappingVM = new NewMappingViewModel(store);
            NodeTreeVM = new NodeTreeModel(store, visualizer);

            MappingErrorVM = new MappingErrorViewModel(MappingVM);
            CalculationVM = new CalculationViewModel(NodeTreeVM);
            SavingVM = new SavingViewModel(CalculationVM);
        }

        public async Task HandleWindowLoadedAsync()
        {
            await LoadingVM.HandleLoadingAsync();
        }

        public void HandleWindowClosing()
        {
            NodeTreeVM.DeselectNodes();
        }

        public async Task HandleProjectSelectedAsync(Project project)
        {
            await LoadingVM.HandleProjectSelectedAsync(project);
            NotifyStoreChange();
        }

        public void HandleForestSelectionChanged(Forest forest)
        {
            ForestVM.HandleForestSelectionChanged(forest);
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            MappingVM.HandleMappingSelectionChanged(mapping);
        }

        public void HandleNewMappingClicked()
        {
            NewMappingVM.HandleNewMappingClicked();
        }

        public async Task HandleNewMappingCreateAsync(Mapping selectedMapping, string newName)
        {
            await NewMappingVM.HandleNewMappingCreate(selectedMapping, newName);
            NotifyStoreChange();
        }

        public void HandleMappingErrorClicked()
        {
            MappingErrorVM.HandleMappingErrorClicked();
        }

        public void HandleBrokenNodeSelectionChanged(NodeModel selectedNode)
        {
            MappingErrorVM.HandleBrokenNodeSelectionChanged(selectedNode);
        }

        public void HandleIgnoreSelectedBrokenNode(NodeModel selectedNode)
        {
            MappingErrorVM.RemoveBrokenNode(selectedNode);
        }

        public void HandleIgnoreAllBrokenNodes()
        {
            MappingErrorVM.RemoveAllBrokenNodes();
        }

        public void HandleErrorMappingSideClicked()
        {
            MappingErrorVM.DeselectNodes();
        }

        public async Task HandleUpdateMapping()
        {
            await MappingVM.HandleUpdateMapping();
        }

        public void HandleNewMappingCanceled()
        {
            NewMappingVM.HandleNewMappingCanceled();
        }

        public void HandleNodeItemSelectionChanged(NodeModel selectedBranch)
        {
            NodeTreeVM.HandleNodeItemSelectionChanged(selectedBranch);
        }

        public void HandleSideClicked()
        {
            NodeTreeVM.DeselectNodes();
        }

        public void HandleInherit()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.NodeBuildupItem.HandleInherit();
        }

        public void HandleRemove()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.NodeBuildupItem.HandleRemove();
        }

        public void HandleViewToggleToBuildup()
        {
            MediatorFromVM.Broadcast("MainViewToggleToBuildup");
        }

        public void HandleViewToggleToBranch()
        {
            MediatorFromVM.Broadcast("MainViewToggleToBranch");
        }

        public void HandleUpdateRevitClicked(Forest forest)
        {
            ForestVM.HandleForestSelectionChanged(forest);
        }

        public void HandleSavingResults()
        {
            SavingVM.HandleSavingResults();
        }

        public async Task HandleSendingResults(string newName)
        {
            await SavingVM.HandleSendingResults(newName);
        }

        public void HandleCancelSavingResults()
        {
            SavingVM.HandleSaveResultCanceled();
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
