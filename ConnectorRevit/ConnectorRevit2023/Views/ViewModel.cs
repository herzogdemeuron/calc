using Calc.ConnectorRevit.Revit;
using Calc.Core;
using Calc.Core.Calculations;
using Calc.Core.Color;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private Store store;
        private Mapping selectedMapping;
        private bool BranchesSwitch = true;
        private readonly ExternalEventHandler eventHandler = new ExternalEventHandler();
        public List<Project> AllProjects { get; set; }
        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }

        public ObservableCollection<NodeViewModel> NodeSource { get=>GetSourceNode(); }

        private NodeViewModel currentForestItem;
        public NodeViewModel CurrentForestItem
        {
            get { return currentForestItem; }
            set
            {
                currentForestItem = value;
                OnPropertyChanged("CurrentForestItem");
            }
        }

        private NodeViewModel selectedNodeItem;
        public NodeViewModel SelectedNodeItem
        {
            get { return selectedNodeItem; }
            set
            {
                selectedNodeItem = value;
                OnPropertyChanged("SelectedNodeItem");
            }
        }

        public Window Window { get; set; }

        private void PlantTrees(Forest forest)
        {
            if (forest == null)
                return;

            foreach (var t in forest.Trees)
            {
                t.Plant(ElementFilter.GetCalcElements(t));
            }
            CurrentForestItem = new NodeViewModel(forest);
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

            Debug.WriteLine("Forest selected: " + forest.Name);
            PlantTrees(forest);
            HandleSideClick();
            OnPropertyChanged("NodeSource");
            ApplyMapping(selectedMapping);
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            ApplyMapping(mapping);
            selectedMapping = mapping;
        }

        private void ApplyMapping(Mapping mapping)
        {
            if (mapping == null)
                return;
            foreach (NodeViewModel nodeItem in CurrentForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                mapping.ApplyMappingToTree(tree, AllBuildups);
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);
            };
            HandleBuildupSelectionChanged();
        }

        public void HandleBuildupSelectionChanged()
        {
            if (BranchesSwitch == false)
            {
                if (CurrentForestItem == null)
                    return;
                Forest currentForest = CurrentForestItem.Host as Forest;
                currentForest.SetBranchColorsBy("buildups");
                CurrentForestItem.NotifyLabelColorChange();
                eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
            }
        }

        public void HandleInherit()
        {
            if (SelectedNodeItem == null)
                return;
            IGraphNode host = SelectedNodeItem.Host;
            //if host is not branch, return
            if (!(host is Branch branch))
                return;
            //if host is branch, inherit

        }
        public void HandleNodeItemSelectionChanged(NodeViewModel nodeItem)
        {

            if (nodeItem == null)
                return;
            SelectedNodeItem = nodeItem;
            HideAllLabelColor();
            if (BranchesSwitch)
            {
                eventHandler.Raise(Visualizer.IsolateAndColorSubbranchElements);
                ShowSubLabelColor(nodeItem);
            }
            else
            {
                eventHandler.Raise(Visualizer.IsolateAndColorBottomBranchElements);
                ShowAllSubLabelColor(nodeItem);
            }
            CurrentForestItem.NotifyLabelColorChange();

        }
        public void HandleSideClick()
        {
            if (CurrentForestItem == null)
                return;
            eventHandler.Raise(Visualizer.Reset);
            HideAllLabelColor();
            EventMessenger.SendMessage("DeselectTreeView");
            SelectedNodeItem = null;
            CurrentForestItem.NotifyLabelColorChange();
        }

        public void HandleViewToggleToBranch()
        {
            if (CurrentForestItem == null)
                return;
            Forest currentForest = CurrentForestItem.Host as Forest;
            BranchesSwitch = true;
            currentForest.SetBranchColorsBy("branches");
            CurrentForestItem.NotifyLabelColorChange();
            HandleSideClick();
        }


        public void HandleViewToggleToBuildup()
        {
            if (CurrentForestItem == null)
                return;
            Forest currentForest = CurrentForestItem.Host as Forest;
            BranchesSwitch = false;
            currentForest.SetBranchColorsBy("buildups");
            CurrentForestItem.NotifyLabelColorChange();
            HandleSideClick();
        }

        public void HandleUpdateCalcElements()
        {
            if (CurrentForestItem == null)
                return;
            Forest currentForest = CurrentForestItem.Host as Forest;
            Mapping currentMapping = GetCurrentMapping();
            PlantTrees(currentForest);
            ApplyMapping(currentMapping);
            HandleSideClick();
        }

        public void HandleCalculate()
        {
            if (CurrentForestItem == null)
                return;
            List<Branch> branchesToCalc = new List<Branch>();
            if (SelectedNodeItem?.Host is Branch branch)
            {
                branchesToCalc.Add(branch);
            }
            else
            {
                foreach (NodeViewModel nodeItem in CurrentForestItem.SubNodeItems)
                {
                    branchesToCalc.Add(nodeItem.Host as Branch);
                }
            }

            List<Result> results = GwpCalculator.CalculateGwp(branchesToCalc);
            Debug.WriteLine("GWP calculated");
        }

        private Mapping GetCurrentMapping()
        {
            if (CurrentForestItem == null)
                return null;
            Forest currentForest = CurrentForestItem.Host as Forest;
            return new Mapping(currentForest, "CurrentMapping");
        }


        private void HideAllLabelColor()
        {
            if (CurrentForestItem == null)
                return;
            foreach (NodeViewModel nodeItem in CurrentForestItem.SubNodeItems)
            {
                HideNodeLabelColor(nodeItem);
            }
        }
        private void HideNodeLabelColor(NodeViewModel nodeItem)
        {
            nodeItem.LabelColorVisible = false;

            foreach (NodeViewModel subNodeItem in nodeItem.SubNodeItems)
            {
                HideNodeLabelColor(subNodeItem);
            }
        }

        private void ShowSubLabelColor(NodeViewModel nodeItem)
        {
            foreach (NodeViewModel subNodeItem in nodeItem.SubNodeItems)
            {
                subNodeItem.LabelColorVisible = true;
            }
        }

        private void ShowAllSubLabelColor(NodeViewModel nodeItem)
        {
            nodeItem.LabelColorVisible = true;
            foreach (NodeViewModel subNodeItem in nodeItem.SubNodeItems)
            {
                subNodeItem.LabelColorVisible = true;
                ShowAllSubLabelColor(subNodeItem);
            }
        }


        private ObservableCollection<NodeViewModel> GetSourceNode()
         {
            if (CurrentForestItem == null)
                return new ObservableCollection<NodeViewModel> { };
            return new ObservableCollection<NodeViewModel> { CurrentForestItem };
    }



public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
