using Calc.Core;
using Calc.Core.Interfaces;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public CalcStore Store { get; set; }
        public ForestViewModel ForestVM { get; set; }
        public MappingViewModel MappingVM { get; set; }
        public MappingErrorViewModel MappingErrorVM { get; set; }
        public NodeTreeModel NodeTreeVM { get; set; }
        public SavingViewModel SavingVM { get; set; }
        public NewMappingViewModel NewMappingVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public CalculationViewModel CalculationVM { get; set; }
        public AssemblySelectionViewModel AssemblySelectionVM { get; set; }

        public MainViewModel(CalcStore store, IElementCreator elementCreator, IVisualizer visualizer)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel();

            ForestVM = new ForestViewModel(store, elementCreator);
            MappingVM = new MappingViewModel(store);
            NewMappingVM = new NewMappingViewModel(store);
            NodeTreeVM = new NodeTreeModel(store, visualizer);

            MappingErrorVM = new MappingErrorViewModel(MappingVM);
            CalculationVM = new CalculationViewModel(NodeTreeVM);
            SavingVM = new SavingViewModel(CalculationVM);

            AssemblySelectionVM = new AssemblySelectionViewModel(store);
        }

        public void HandleWindowLoaded()
        {
            OnPropertyChanged(nameof(Store));
        }

        public void HandleWindowClosing()
        {
            NodeTreeVM.DeselectNodes();
        }

        public async void HandleForestSelectionChanged(Forest forest)
        {
            if (Store.ForestSelected == forest) return; 
            await ForestVM.HandleForestSelectionChanged(forest);
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
            OnPropertyChanged(nameof(Store));
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

        public bool HandleSelectingAssembly(bool setMain)
        {
            if (NodeTreeVM.SelectedNodeItem == null) return false;
            var assemblyItem = NodeTreeVM.SelectedNodeItem.NodeAssemblyItem;
            var assembly = setMain ? assemblyItem.Assembly1 : assemblyItem.Assembly2;
            AssemblySelectionVM.PrepareAssemblySelection(assembly);
            return true;
        }

        public void HandleAssemblySelected(bool setMain)
        {
            var assembly = AssemblySelectionVM.SelectedAssembly;
            NodeTreeVM.SelectedNodeItem.NodeAssemblyItem.SetAssembly(setMain,assembly);
        }

        public void HandleSideClicked()
        {
            NodeTreeVM.DeselectNodes();
        }

        public void HandleInherit()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.NodeAssemblyItem.HandleInherit();
        }

        public void HandleRemove()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.NodeAssemblyItem.HandleRemove();
        }

        public void HandleViewToggleToAssembly()
        {
            MediatorFromVM.Broadcast("MainViewToggleToAssembly");
        }

        public void HandleViewToggleToBranch()
        {
            MediatorFromVM.Broadcast("MainViewToggleToBranch");
        }

        public async void HandleUpdateRevitClicked(Forest forest)
        {
            await ForestVM.HandleForestSelectionChanged(forest);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
