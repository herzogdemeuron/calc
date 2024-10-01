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
        public NodeTreeViewModel NodeTreeVM { get; set; }
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
            AssemblySelectionVM = new AssemblySelectionViewModel(store);
            NodeTreeVM = new NodeTreeViewModel(store, visualizer);
            MappingErrorVM = new MappingErrorViewModel(MappingVM);
            CalculationVM = new CalculationViewModel(NodeTreeVM);
            SavingVM = new SavingViewModel(CalculationVM);
        }

        /// <summary>
        /// actions be taken when the mapping setting or the node source is changed
        /// </summary>
        private void MappingChangedActions()
        {
            var brokenForest = NodeTreeVM.ReMapAllNodes();
            MappingErrorVM.UpdateBrokenNodes(brokenForest);
            AssemblySelectionChangedActions(true);
        }

        private void AssemblySelectionChangedActions(bool all = false)
        {
            // refresh the assembly section ui
            var node = all ? NodeTreeVM.CurrentForestItem : NodeTreeVM.SelectedNodeItem;
            node.RefreshAssemblySection();
            NodeTreeVM.ReColorAllNodes(true);
            CalculationVM.RefreshCalculation();
        }

        public void HandleWindowClosing()
        {
            NodeTreeVM.DeselectNodes();
        }

        public async void HandleForestSelectionChanged(Forest forest, bool forceUpdate = false)
        {
            if (forest == null) return;
            if(forest == Store.ForestSelected && !forceUpdate) return;
            await ForestVM.HandleForestSelectionChanged(forest);
            NodeTreeVM.UpdateNodeSource();
            MappingChangedActions();
            NodeTreeVM.DeselectNodes();
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {            
            MappingChangedActions();
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

        public void HandleNodeItemSelectionChanged(NodeModel selectedNode)
        {
            NodeTreeVM.HandleNodeItemSelectionChanged(selectedNode);
            selectedNode.AssemblyModel.SetFirstAssemblyToActive();
            CalculationVM.RefreshCalculation();
        }

        public bool HandleSelectingAssembly(bool setMain)
        {
            if (NodeTreeVM.SelectedNodeItem == null) return false;
            var assemblyItem = NodeTreeVM.SelectedNodeItem.AssemblyModel;
            var assembly = setMain ? assemblyItem.Assembly1 : assemblyItem.Assembly2;
            AssemblySelectionVM.PrepareAssemblySelection(assembly);
            return true;
        }

        public void HandleAssemblySelected(bool setMain)
        {
            var assembly = AssemblySelectionVM.SelectedAssembly;
            NodeTreeVM.SelectedNodeItem.SetAssembly(setMain,assembly);
            AssemblySelectionChangedActions();
        }

        public void HandleSideClicked()
        {
            NodeTreeVM.DeselectNodes();
            CalculationVM.RefreshCalculation();
        }

        public void HandleInherit()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.AssemblyModel.HandleInherit();
            AssemblySelectionChangedActions();
        }

        public void HandleRemove()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.AssemblyModel.HandleRemove();
            AssemblySelectionChangedActions();
        }

        public void HandleViewToggleToAssembly()
        {
            NodeTreeVM.ColorNodesToAssembly();
        }

        public void HandleViewToggleToBranch()
        {
            NodeTreeVM.ColorNodesToBranch();
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
