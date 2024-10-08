using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.MVVM.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// The main view model for calc project.
    /// </summary>
    public class ProjectViewModel: INotifyPropertyChanged
    {
        public CalcStore Store { get; set; }
        public QueryTemplateViewModel QueryTemplateVM { get; set; }
        public MappingViewModel MappingVM { get; set; }
        public MappingErrorViewModel MappingErrorVM { get; set; }
        public NodeTreeViewModel NodeTreeVM { get; set; }
        public SavingViewModel SavingVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public CalculationViewModel CalculationVM { get; set; }
        public AssemblySelectionViewModel AssemblySelectionVM { get; set; }
        public event EventHandler DeselectTreeView;
        public event EventHandler DeselectBrokenQueryView;

        public ProjectViewModel(CalcStore store, IElementCreator elementCreator, IVisualizer visualizer)
        {
            Store = store;
            VisibilityVM = new VisibilityViewModel();
            QueryTemplateVM = new QueryTemplateViewModel(store, elementCreator, VisibilityVM);
            MappingVM = new MappingViewModel(store, VisibilityVM);
            AssemblySelectionVM = new AssemblySelectionViewModel(store);
            NodeTreeVM = new NodeTreeViewModel(store, visualizer);
            MappingErrorVM = new MappingErrorViewModel(MappingVM);
            CalculationVM = new CalculationViewModel(NodeTreeVM);
            SavingVM = new SavingViewModel(CalculationVM, VisibilityVM);
        }

        /// <summary>
        /// Actions be taken when the mapping setting or the node source is changed.
        /// </summary>
        private void MappingChangedActions()
        {
            var brokenQuerySet = NodeTreeVM.ReMapAllNodes();
            MappingErrorVM.UpdateBrokenNodes(brokenQuerySet);
            AssemblySelectionChangedActions(true);
        }

        /// <summary>
        /// Actions be taken when the assembly selection is changed.
        /// </summary>
        private void AssemblySelectionChangedActions(bool all = false)
        {
            // refresh the assembly section ui
            var node = all ? NodeTreeVM.CurrentQueryTemplateItem : NodeTreeVM.SelectedNodeItem;
            node.RefreshAssemblySection();
            NodeTreeVM.ReColorAllNodes(true);
            CalculationVM.RefreshCalculation();
        }

        internal void HandleWindowClosing()
        {
            NodeTreeVM.DeselectNodes();
        }

        internal async void HandleQueryTemplateSelectionChanged(QueryTemplate qryTemplate, bool forceUpdate = false)
        {
            if (qryTemplate == null) return;
            if(qryTemplate == Store.QueryTemplateSelected && !forceUpdate) return;
            await QueryTemplateVM.HandleQueryTemplateSelectionChanged(qryTemplate);
            NodeTreeVM.UpdateNodeSource();
            MappingChangedActions();
            NodeTreeVM.DeselectNodes();
            DeselectTreeView?.Invoke(this, EventArgs.Empty);
        }

        public void HandleMappingSelectionChanged()
        {            
            MappingChangedActions();
        }

        public void HandleNewMappingClicked()
        {
            MappingVM.HandleNewMappingClicked();
        }

        public async Task HandleNewMappingCreateAsync(Mapping selectedMapping, string newName)
        {
            await MappingVM.HandleNewMappingCreate(selectedMapping, newName);
            OnPropertyChanged(nameof(Store));
        }

        public void HandleMappingErrorClicked()
        {
            MappingErrorVM.HandleMappingErrorClicked();
        }

        internal void HandleBrokenNodeSelectionChanged(NodeModel selectedNode)
        {
            MappingErrorVM.HandleBrokenNodeSelectionChanged(selectedNode);
        }

        internal void HandleIgnoreSelectedBrokenNode(NodeModel selectedNode)
        {
            MappingErrorVM.RemoveBrokenNode(selectedNode);
        }

        internal void HandleIgnoreAllBrokenNodes()
        {
            MappingErrorVM.RemoveAllBrokenNodes();
        }

        internal void HandleErrorMappingSideClicked()
        {
            DeselectBrokenQueryView?.Invoke(this, EventArgs.Empty);
        }

        internal async Task HandleUpdateMapping()
        {
            await MappingVM.HandleUpdateMapping();
        }

        public void HandleNewMappingCanceled()
        {
            MappingVM.HandleNewMappingCanceled();
        }

        internal void HandleNodeItemSelectionChanged(NodeModel selectedNode)
        {
            NodeTreeVM.HandleNodeItemSelectionChanged(selectedNode);
            selectedNode.AssemblyModel.SetFirstAssemblyToActive();
            CalculationVM.RefreshCalculation();
        }

        internal bool HandleSelectingAssembly(bool setMain)
        {
            if (NodeTreeVM.SelectedNodeItem == null) return false;
            var assemblyItem = NodeTreeVM.SelectedNodeItem.AssemblyModel;
            var assembly = setMain ? assemblyItem.Assembly1 : assemblyItem.Assembly2;
            AssemblySelectionVM.PrepareAssemblySelection(assembly);
            return true;
        }

        internal void HandleAssemblySelected(bool setMain)
        {
            var assembly = AssemblySelectionVM.SelectedAssembly;
            NodeTreeVM.SelectedNodeItem.SetAssembly(setMain,assembly);
            AssemblySelectionChangedActions();
        }

        internal void HandleSideClicked()
        {
            NodeTreeVM.DeselectNodes();
            CalculationVM.RefreshCalculation();
            DeselectTreeView?.Invoke(this, EventArgs.Empty);
        }

        internal void HandleInherit()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.AssemblyModel.HandleInherit();
            AssemblySelectionChangedActions();
        }

        internal void HandleRemove()
        {
            if (NodeTreeVM.SelectedNodeItem == null)
                return;
            NodeTreeVM.SelectedNodeItem.AssemblyModel.HandleRemove();
            AssemblySelectionChangedActions();
        }

        internal void HandleViewToggleToAssembly()
        {
            NodeTreeVM.ColorNodesToAssembly();
        }

        internal void HandleViewToggleToBranch()
        {
            NodeTreeVM.ColorNodesToBranch();
        }

        internal void HandleSavingResults()
        {
            SavingVM.HandleSavingResults();
        }

        internal async Task HandleSendingResults(string newName)
        {
            await SavingVM.HandleSendingResults(newName);
        }

        internal void HandleCancelSavingResults()
        {
            SavingVM.HandleSaveResultCanceled();
        }
        internal void HandleMessageClose()
        {
            VisibilityVM.HideMessageOverlay();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
