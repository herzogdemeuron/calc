using Calc.ConnectorRevit.Revit;
using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private Store store;
        private Project selectedProject;
        public Forest SelectedForest { get; set; }
        private Mapping selectedMapping;
        private bool showBranches = true;
        
        public ObservableCollection<BranchViewModel> ForestList
        {
            get
            {
                return ForestItem != null
                    ? new ObservableCollection<BranchViewModel> { ForestItem }
                    : null;
            }
        }

        public List<Project> AllProjects { get; set; }
        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }

        private readonly ExternalEventHandler eventHandler;

        private BranchViewModel forestItem;
        public BranchViewModel ForestItem
        {
            get { return forestItem; }
            set
            {
                forestItem = value;
                OnPropertyChanged("ForestItem");
                OnPropertyChanged("ForestList");
            }
        }

        private ObservableCollection<BranchViewModel> branchItems;
        public ObservableCollection<BranchViewModel> BranchItems
        {
            get { return branchItems; }
            set
            {
                branchItems = value;
                OnPropertyChanged("BranchItems");
            }
        }

        private BranchViewModel selectedBranchItem;
        public BranchViewModel SelectedBranchItem
        {
            get { return selectedBranchItem; }
            set
            {
                selectedBranchItem = value;
                OnPropertyChanged("SelectedBranchItem");
            }
        }

        public ViewModel()
        {
            eventHandler = new ExternalEventHandler();
            
        }

        private void PlantTrees()
        {
            if (SelectedForest == null)
                return;
            
            var newBranchItems = new ObservableCollection<BranchViewModel>();
            foreach (var t in SelectedForest.Trees)
            {
                t.Plant(ElementFilter.GetCalcElements(t));
                newBranchItems.Add(new BranchViewModel(t));
            }
            BranchItems = newBranchItems;
            
            ForestItem = new BranchViewModel(selectedProject.ProjectNumber, BranchItems);
        }

        public async Task HandleLoadingAsync()
        {
            store = new Store();
            await store.GetProjects();
            AllProjects = store.ProjectsAll;
            OnPropertyChanged("AllProjects");
        }

        public async Task HandleProjectSelectedAsync(Project project)
        {
            store.ProjectSelected = project;
            selectedProject = project;
            await store.GetOtherData();

            AllBuildups = store.BuildupsAll;
            AllForests = store.Forests;
            AllMappings = store.MappingsAll;
            
            OnPropertyChanged("AllBuildups");
            OnPropertyChanged("AllForests");
            OnPropertyChanged("AllMappings");
        }

        public void HandleForestSelectionChanged(Forest forest)
        {
            if (forest == null)
                return;
            SelectedForest = forest;
            PlantTrees();
            ApplyMapping(selectedMapping);
            HandleSideClick();
        }

        private void ApplyMapping(Mapping mapping)
        {
            if (mapping == null)
                return;
            foreach (BranchViewModel branchItem in BranchItems)
            {
                Tree tree = branchItem.Branch as Tree;
                mapping.ApplyMappingToTree(tree, AllBuildups);
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);
            };
            HandleBuildupSelectionChanged();
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            ApplyMapping(mapping);
            selectedMapping = mapping;
        }

        public void HandleBuildupSelectionChanged()
        {
            if (showBranches == false)
            {
                SelectedForest.SetBranchColorsBy("buildups");
                ForestItem.NotifyLabelColorChange();
                eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
            }
        }

        public void HandleBranchSelectionChanged(BranchViewModel branchItem)
        {
            if (branchItem == null)
                return;

            HideAllLabelColor();
            SelectedBranchItem = branchItem;

            if (showBranches)
            {
                eventHandler.Raise(Visualizer.IsolateAndColorSubbranchElements);
                ShowSubLabelColor(branchItem);
            }
            else
            {
                eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
                ShowAllSubLabelColor(branchItem);
            }

        }
        public void HandleSideClick()
        {
            eventHandler.Raise(Visualizer.Reset);
            HideAllLabelColor();
            EventMessenger.SendMessage("DeselectTreeView");
        }


        public void HandleViewToggleToBranch()
        {
            showBranches = true;
            SelectedForest.SetBranchColorsBy("branches");
            ForestItem.NotifyLabelColorChange();
            HandleSideClick();
        }


        public void HandleViewToggleToBuildup()
        {
            showBranches = false;
            SelectedForest.SetBranchColorsBy("buildups");
            ForestItem.NotifyLabelColorChange();
            HandleSideClick();
        }

        public void HandleUpdateCalcElements()
        {
            Mapping currentMapping = GetCurrentMapping();
            PlantTrees();
            ApplyMapping(currentMapping);
            HandleSideClick();
        }
            

        private Mapping GetCurrentMapping()
        {
            if (SelectedForest == null)
                return null;
            return new Mapping(SelectedForest, "CurrentMapping");
        }


        private void HideAllLabelColor()
        {
            foreach (BranchViewModel item in BranchItems)
            {
                HideBranchLabelColor(item);
            }
        }
        private void HideBranchLabelColor(BranchViewModel branchItem)
        {
            branchItem.ShowLabelColor = false;

            foreach (BranchViewModel childBranchItem in branchItem.SubBranchItems)
            {
                HideBranchLabelColor(childBranchItem);
            }
        }

        private void ShowSubLabelColor(BranchViewModel branchItem)
        {
            foreach (BranchViewModel childBranchItem in branchItem.SubBranchItems)
            {
                childBranchItem.ShowLabelColor = true;
            }
        }

        private void ShowAllSubLabelColor(BranchViewModel branchItem)
        {
            branchItem.ShowLabelColor = true;
            foreach (BranchViewModel childBranchItem in branchItem.SubBranchItems)
            {
                childBranchItem.ShowLabelColor = true;
                ShowAllSubLabelColor(childBranchItem);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
