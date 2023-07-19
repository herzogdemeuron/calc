using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Services;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

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

        public MainViewModel(DirectusStore store)
        {
            Store = store;
            Server = new LiveServer();
            LoadingVM = new LoadingViewModel(store);
            ForestVM = new ForestViewModel(store);
            MappingVM = new MappingViewModel(store);
            BuildupVM = new BuildupViewModel();
            NodeTreeVM = new NodeTreeViewModel(store);
        }

        public void NotifyStoreChange()
        {
            OnPropertyChanged(nameof(Store));
        }

        public void HandleViewToggleToBuildup()
        {
            Mediator.Broadcast("ViewToggleToBuildup");
        }

        public void HandleViewToggleToBranch()
        {
            Mediator.Broadcast("ViewToggleToBranch");
        }

        public void HandleStartCalcLive()
        {
            Server.Start();
        }

        public void HandleSaveResults()
        {
            //Mediator.Broadcast("SaveResults");
        }

        public void HandleWindowClosing()
        {
            Server.Stop();
        }

         public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
