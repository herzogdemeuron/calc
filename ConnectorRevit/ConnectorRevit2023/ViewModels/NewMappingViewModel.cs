using Calc.Core;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.Generic;
using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Views;
using Autodesk.Revit.DB;
using System;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NewMappingViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;


        private string newName;
        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        private ICollectionView mappingsListView;
        public ICollectionView MappingsListView
        {
            get { return mappingsListView; }
            set
            {
                mappingsListView = value;
                OnPropertyChanged(nameof(MappingsListView));
            }
        }
        public NewMappingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
        }
        public void HandleNewMappingClicked()
        {
            CreateMappingsList();
            ViewMediator.Broadcast("ShowNewMappingOverlay");
        }

/*        public async Task HandleNewMappingCreate()
        {
            //Window newMappingWindow = new NewMappingView(store);
            //newMappingWindow.ShowDialog();
            await store.SaveSelectedMapping();
            Mediator.Broadcast("MappingSelectionChanged", store.MappingSelected);
        }*/
        
        private void CreateMappingsList()
        {
            List<Mapping> allMappings = new List<Mapping>(store.MappingsAll);
            MappingsListView = CollectionViewSource.GetDefaultView(allMappings);
            MappingsListView.GroupDescriptions?.Add(new PropertyGroupDescription("Project.ProjectNumber"));
        }


        public async Task HandleNewMappingCreate(Mapping selectedMapping, string newName)
        {
            bool? feedback;
            string error = "";

            if (selectedMapping == null) 
            {
                ViewMediator.Broadcast("ShowMessageOverlay", new List<object> { null, "Please choose a mapping to duplicate." });
                return;
            }
            
            if (string.IsNullOrEmpty(newName))
            {
                ViewMediator.Broadcast("ShowMessageOverlay", new List<object> { null, "Please enter a new name." });
                return;
            }

            List<string> currentNames = store.MappingsProjectRelated.Select(m => m.Name).ToList();
            if (currentNames.Contains(newName))
            {
                ViewMediator.Broadcast("ShowMessageOverlay", new List<object> { null, "Name already exists in current project." });
                return;
            }

            try
            {
                ViewMediator.Broadcast("ShowWaitingOverlay", "Creating new mapping...");
                Mapping newMapping = selectedMapping.Copy(newName);
                store.MappingSelected = newMapping;
                feedback = await store.SaveSelectedMapping();
                Mediator.Broadcast("MappingSelectionChanged", store.MappingSelected);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            ViewMediator.Broadcast("ShowMainView");
            ViewMediator.Broadcast
                ("ShowMessageOverlay",
                new List<object>
                    {  
                    feedback,
                    error,
                    "Created mapping successfully.",
                    "Error occured while creating."
                    }
                );
        }

        public void HandleNewMappingCanceled()
        {
            ViewMediator.Broadcast("ShowMainView");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
