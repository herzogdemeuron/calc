using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Calc.MVVM.ViewModels
{

    public class LoadingViewModel : INotifyPropertyChanged  // deprecated
    {
        private readonly CalcStore store;
        public List<CalcProject> AllProjects { get => store?.ProjectsAll;}

        public LoadingViewModel(CalcStore calcStore)
        {
            store = calcStore;
        }

        public async Task HandleProjectSelectedAsync(CalcProject project)
        {
            if (project == null)
            {
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object>{null, "Please choose a project."});
                return;
            }
            MediatorToView.Broadcast("ShowWaitingOverlay", "Loading project data...");
            store.ProjectSelected = project;

            try
            {
                bool dataGot = await store.GetMainData(new CancellationTokenSource().Token); //todo: add cancellation token source 
                MediatorToView.Broadcast("ShowMainView");

            }
            catch (Exception ex)
            {
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, $"Error occured while getting project data:{ex.Message}" });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}