using Calc.ConnectorRevit.Revit;
using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private Store store;
        private Forest selectedForest;
        private Mapping selectedMapping;
        private bool showBranches = true;
        public List<Project> AllProjects { get; set; }
        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }

        private readonly ExternalEventHandler eventHandler;


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
            selectedForest = forest;

            var newBranchItems = new ObservableCollection<BranchViewModel>();
            foreach (var t in forest.Trees)
            {
                t.Plant(ElementFilter.GetCalcElements(t));
                newBranchItems.Add(new BranchViewModel(t));
            }
            BranchItems = newBranchItems;
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            if (mapping == null)
                return;
            selectedMapping = mapping;
            foreach (BranchViewModel branchItem in BranchItems)
            {
                Tree tree = branchItem.Branch as Tree;
                mapping.ApplyMappingToTree(tree, AllBuildups);
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);
                Debug.WriteLine($"Set mappings to tree: {tree.Name}");
            };           
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

        public void HandleViewToggleToBuildup()
        {
            showBranches = false;
            foreach (BranchViewModel branchItem in BranchItems)
            {
                Tree tree = branchItem.Branch as Tree;
                BranchPainter.ColorBranchesByBuildup(tree.SubBranches);
                branchItem.NotifyLabelColorChange();
            };
            HandleSideClick();
        }

        public void HandleViewToggleToBranch()
        {
            showBranches = true;
            foreach (BranchViewModel branchItem in BranchItems)
            {
                Tree tree = branchItem.Branch as Tree;
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);
                branchItem.NotifyLabelColorChange();
            };
            HandleSideClick();
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
