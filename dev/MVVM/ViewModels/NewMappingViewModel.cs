using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Calc.MVVM.ViewModels
{
    public class NewMappingViewModel : INotifyPropertyChanged
    {
        private CalcStore store;


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
        public NewMappingViewModel(CalcStore calcStore)
        {
            store = calcStore;
        }
        public void HandleNewMappingClicked()
        {
            CreateMappingsList();
            MediatorToView.Broadcast("ShowNewMappingOverlay");
        }
        
        private void CreateMappingsList()
        {
            List<Mapping> allMappings = new List<Mapping>(store.MappingsAll);
            MappingsListView = CollectionViewSource.GetDefaultView(allMappings);
            MappingsListView.GroupDescriptions?.Add(new PropertyGroupDescription("Project.Number"));
        }


        public async Task HandleNewMappingCreate(Mapping selectedMapping, string newName)
        {
            bool? feedback;
            string error = "";

/*            if (selectedMapping == null) 
            {
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, "Please choose a mapping to duplicate." });
                return;
            }*/
            
            if (string.IsNullOrEmpty(newName))
            {
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, "Please enter a new name." });
                return;
            }

            List<string> currentNames = store.MappingsProjectRelated.Select(m => m.Name).ToList();
            if (currentNames.Contains(newName))
            {
                MediatorToView.Broadcast("ShowMessageOverlay", new List<object> { null, "Name already exists in current project." });
                return;
            }

            try
            {
                MediatorToView.Broadcast("ShowWaitingOverlay", "Creating new mapping...");
                // choose if save current mapping or duplicate from selected
                Mapping newMapping;
                // if no mapping selected, create new mapping from current forest
                if (selectedMapping == null)
                {
                    newMapping = new Mapping(newName, store.ForestSelected);
                }
                else
                {
                    newMapping = selectedMapping.Copy(newName);
                }
                feedback = await store.CreateMapping(newMapping);
                MediatorFromVM.Broadcast("MappingSelectionChanged", store.MappingSelected);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            MediatorToView.Broadcast("ShowMainView");
            MediatorToView.Broadcast
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
            MediatorToView.Broadcast("ShowMainView");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
