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
            OnPropertyChanged("AllBuildups");
            AllForests = store.Forests;
            OnPropertyChanged("AllForests");
            AllMappings = store.MappingsAll;
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

        public void HandleSideClick()
        {
            eventHandler.Raise(Visualizer.Reset);
            foreach (BranchViewModel item in BranchItems)
            {
                RemoveDisplayColor(item);
            }
        }

        public void HandleBranchSelectionChanged(BranchViewModel branchItem)
        {
            if (branchItem == null)
                return;

            foreach (BranchViewModel item in BranchItems)
            {
                RemoveDisplayColor(item);
            }

            SelectedBranchItem = branchItem;
            eventHandler.Raise(Visualizer.IsolateAndColor);
            ResetDisplayColor(branchItem);
        }

        private void RemoveDisplayColor(BranchViewModel branchItem)
        {
            branchItem.DisplayColor = false;

            foreach (BranchViewModel childBranchItem in branchItem.SubBranchItems)
            {
                RemoveDisplayColor(childBranchItem);
            }
        }

        private void ResetDisplayColor(BranchViewModel branchItem)
        {
            foreach (BranchViewModel childBranchItem in branchItem.SubBranchItems)
            {
                childBranchItem.DisplayColor = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
