using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Services;
using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public DirectusStore Store { get; set; }
        public  LiveServer Server { get; set; }
        public LoadingViewModel LoadingVM { get; set; }
        public ForestViewModel ForestVM { get; set; }
        public MappingViewModel MappingVM { get; set; }
        public BuildupViewModel BuildupVM { get; set; }
        public NodeTreeViewModel NodeTreeVM { get; set; }
        public SavingViewModel SavingVM { get; set; }
        public NewMappingViewModel NewMappingVM { get; set; }
        public VisibilityViewModel VisibilityVM { get; set; }
        public CalculationViewModel CalculationVM { get; set; }

        public MainViewModel(DirectusStore store)
        {
            Store = store;
            Server = new LiveServer();
            LoadingVM = new LoadingViewModel(store);
            ForestVM = new ForestViewModel(store);
            MappingVM = new MappingViewModel(store);
            NewMappingVM = new NewMappingViewModel(store);

            
            NodeTreeVM = new NodeTreeViewModel(store);
            BuildupVM = new BuildupViewModel(NodeTreeVM);
            SavingVM = new SavingViewModel(NodeTreeVM);
            CalculationVM = new CalculationViewModel(NodeTreeVM);


            VisibilityVM = new VisibilityViewModel();
        }

        public async Task HandleWindowLoadedAsync()
        {
            await LoadingVM.HandleLoadingAsync();
        }

        public void HandleWindowClosing()
        {
            NodeTreeVM.DeselectNodes();
            Server.Stop();
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

        public async Task HandleUpdateMapping()
        {
            await MappingVM.HandleUpdateMapping();
        }

        public void HandleNewMappingCanceled()
        {
            NewMappingVM.HandleNewMappingCanceled();
        }

        public void HandleNodeItemSelectionChanged(NodeViewModel selectedBranch)
        {
            NodeTreeVM.HandleNodeItemSelectionChanged(selectedBranch);
        }

        public void HandleSideClicked()
        {
            NodeTreeVM.DeselectNodes();
        }

        public void HandleInherit()
        {
            BuildupVM.HandleInherit();
        }

        public void HandleRemove()
        {
            BuildupVM.HandleRemove();
        }

        public void HandleViewToggleToBuildup()
        {
            Mediator.Broadcast("ViewToggleToBuildup");
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        public void HandleViewToggleToBranch()
        {
            Mediator.Broadcast("ViewToggleToBranch");
            Mediator.Broadcast("BuildupSelectionChanged");
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
            ViewMediator.Broadcast("HideMessageOverlay");
        }

        public void NotifyStoreChange()
        {
            OnPropertyChanged(nameof(Store));
        }

        public void HandleStartCalcLive()
        {
            Server.Start();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
