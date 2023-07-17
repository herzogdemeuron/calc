using Calc.ConnectorRevit.Services;
using Calc.Core;
using Calc.Core.DirectusAPI;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Helpers;

namespace Calc.ConnectorRevit.ViewModels
{

    public class LoadingViewModel : INotifyPropertyChanged
    {
        private readonly DirectusStore store;
        public List<Project> AllProjects { get => store?.ProjectsAll;}

        public LoadingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }

        public async Task HandleProjectSelectedAsync(Project project)
        {
            store.ProjectSelected = project;
            await store.GetOtherData();
            Mediator.Broadcast("ProjectSelected");
        }

        public async Task HandleLoadingAsync()
        {

            await store.GetProjects();
            OnPropertyChanged(nameof(AllProjects));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}