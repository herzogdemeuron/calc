using Calc.ConnectorRevit.Revit;
using Calc.Core;
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

        private List<Project> projects;
        public List<Project> Projects
        {
            get { return projects; }
            set
            {
                projects = value;
                OnPropertyChanged("Projects");
            }
        }

        private List<Buildup> allBuildups;
        public List<Buildup> AllBuildups
        {
            get { return allBuildups; }
            set
            {
                allBuildups = value;
                OnPropertyChanged("AllBuildups");
            }
        }

        private List<Forest> forests;
        public List<Forest> Forests
        {
            get { return forests; }
            set
            {
                forests = value;
                OnPropertyChanged("Forests");
            }
        }

        private List<Mapping> allMappings;
        public List<Mapping> AllMappings
        {
            get { return allMappings; }
            set
            {
                allMappings = value;
                OnPropertyChanged("AllMappings");
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

        private List<Branch> currentTrees;

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

        private Forest selectedForest;
        public Forest SelectedForest
        {
            get { return selectedForest; }
            set
            {
                selectedForest = value;
                OnPropertyChanged("SelectedForest");
            }
        }

        private Store store;
        private readonly ExternalEventHandler eventHandler;
        private Mapping selectedMapping;

        public ViewModel()
        {
            eventHandler = new ExternalEventHandler();
        }
        public async Task HandleLoadingAsync()
        {
            store = new Store();
            await store.GetProjects();
            Projects = store.ProjectsAll; 
        }

        public async Task HandleProjectSelectedAsync(Project project)
        {
            store.ProjectSelected = project;
            await store.GetOtherData();
            AllBuildups = store.BuildupsAll;
            Forests = store.Forests;
            AllMappings = store.MappingsAll;
        }

        public void HandleForestSelectionChanged(Forest forest)
        {
            if (forest == null)
                return;
            SelectedForest = forest;

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
                branchItem.UpdateBuildups();
                Debug.WriteLine($"Set mappings to tree: {tree.Name}");
            };           
        }
        public void HandleBranchSelectionChanged(BranchViewModel branchItem)
        {
            if (branchItem == null)
                return;
            SelectedBranchItem = branchItem;
            eventHandler.Raise(Visualizer.IsolateAndColor);
        }

        public void HandleSideClick()
        {
            eventHandler.Raise(Visualizer.Reset);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
