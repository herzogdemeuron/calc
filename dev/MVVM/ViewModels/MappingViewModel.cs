﻿using Calc.Core;
using Calc.Core.Objects.GraphNodes;
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
    /// <summary>
    /// Handles updating and creating new mappings.
    /// </summary>
    public class MappingViewModel : INotifyPropertyChanged
    {
        private readonly CalcStore store;
        private readonly VisibilityViewModel visibilityVM;
        public QueryTemplate BrokenQuerySet { get; set; }
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

        public MappingViewModel(CalcStore calcStore, VisibilityViewModel vvm)
        {
            store = calcStore;
            visibilityVM = vvm;
        }

        internal async Task HandleUpdateMapping()
        {
            bool? feedback;
            string error = "";
            try
            {
                visibilityVM.ShowWaitingOverlay("Updating mapping...");
                feedback = await store.UpdateSelectedMapping(BrokenQuerySet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            visibilityVM.HideAllOverlays();
            visibilityVM.ShowMessageOverlay(
                        feedback,
                        error,
                        "Updated mapping successfully.",
                        "Error occured while saving, please try again."
                        );
        }

        internal void HandleNewMappingClicked()
        {
            CreateMappingsList();
            visibilityVM.ShowNewMappingOverlay();
        }
        
        private void CreateMappingsList()
        {
            List<Mapping> allMappings = new List<Mapping>(store.MappingsAll);
            MappingsListView = CollectionViewSource.GetDefaultView(allMappings);
            MappingsListView.GroupDescriptions?.Add(new PropertyGroupDescription("Project.Number"));
        }

        internal async Task HandleNewMappingCreate(Mapping selectedMapping, string newName)
        {
            bool? feedback;
            string error = "";            
            if (string.IsNullOrEmpty(newName))
            {
                visibilityVM.ShowMessageOverlay(null, "Please enter a new name.");
                return;
            }
            List<string> currentNames = store.RelatedMappings.Select(m => m.Name).ToList();
            if (currentNames.Contains(newName))
            {
                visibilityVM.ShowMessageOverlay(null, "Name already exists in current project.");
                return;
            }
            try
            {
                visibilityVM.ShowWaitingOverlay("Creating new mapping...");
                // choose if save current mapping or duplicate from selected
                Mapping newMapping;
                // if no mapping selected, create new mapping from current query template
                if (selectedMapping == null)
                {
                    newMapping = new Mapping(newName, store.QueryTemplateSelected);
                }
                else
                {
                    newMapping = selectedMapping.Copy(newName);
                }
                feedback = await store.CreateMapping(newMapping);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            visibilityVM.HideAllOverlays();
            visibilityVM.ShowMessageOverlay(
                    feedback,
                    error,
                    "Created mapping successfully.",
                    "Error occured while creating."
                    );
        }

        /// <summary>
        /// Set back the visibilities.
        /// </summary>
        internal void HandleNewMappingCanceled()
        {
            visibilityVM.HideAllOverlays();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
