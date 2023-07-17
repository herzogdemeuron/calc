using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Calc.ConnectorRevit.ViewModels
{
    public class ViewModel: INotifyPropertyChanged
    {
        public DirectusStore Store { get; set; }
        public LoadingViewModel LoadingVM { get; set; }
        public ForestViewModel ForestVM { get; set; }
        public MappingViewModel MappingVM { get; set; }
        public BuildupViewModel BuildupVM { get; set; }
        public NodeTreeViewModel NodeTreeVM { get; set; }

        public bool BranchesSwitch = true;
        public List<Forest> CurrentForests
        {
            get
            {
                return Store.ForestProjectRelated;
            }
        }
        public ViewModel(DirectusStore store)
        {
            Store = store;
            LoadingVM = new LoadingViewModel(store);
            ForestVM = new ForestViewModel(store);
            MappingVM = new MappingViewModel(store);
            BuildupVM = new BuildupViewModel();
            NodeTreeVM = new NodeTreeViewModel(store, BranchesSwitch);    
            App.NodeTreeVM = NodeTreeVM;
            Mediator.Register("ProjectSelected", _ => NotifyUI());
        }

        private void NotifyUI()
        {
            OnPropertyChanged(nameof(Store));
            Debug.Assert(App.View != null);
        }



        public void HandleViewToggleToBuildup()
        {
            BranchesSwitch = false;
            Mediator.Broadcast("ViewToggleToBuildup");
        }

        public void HandleViewToggleToBranch()
        {
            BranchesSwitch = true;
            Mediator.Broadcast("ViewToggleToBranch");
        }

        public void HandleUpdateCalcElements()
        {
            Mediator.Broadcast("UpdateCalcElements");
        }

         public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
