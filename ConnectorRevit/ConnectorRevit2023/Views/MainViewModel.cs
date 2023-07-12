using System;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Calc.Core.Calculations;
using Calc.ConnectorRevit.Revit;


namespace Calc.ConnectorRevit.Views
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private DirectusStore store;
        private CalcWebSocketServer server;
        private bool BranchesSwitch = true;
        private readonly ExternalEventHandler eventHandler = new ExternalEventHandler();
        public List<Project> AllProjects { get; set; }
        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> Forests
        {
            get => store?.ForestProjectRelated;
        }
        public List<Mapping> Mappings
        {
            get => store?.MappingsProjectRelated;
        }

        public Mapping MappingSelected
        {
            get => store.MappingSelected;
            set
            {
                store.MappingSelected = value;
                OnPropertyChanged("MappingSelected");
            }
        }
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

        public MainViewModel()
        {
            this.server = new CalcWebSocketServer("http://127.0.0.1:8184/");
            _ = this.server.Start();
        }

        public void Dispose()
        {
            Debug.WriteLine("ViewModel dispose is called.");
            _ = Task.Run(async () => await this.server.Stop());
        }

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
            var directus = null as Directus;
            try
            {
                var authenticator = new DirectusAuthenticator();
                directus = await authenticator.ShowLoginWindowAsync();
            }
             catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (directus == null)
            {
                Window.Close();
                return;
            }

            store = new DirectusStore(directus);
            Debug.WriteLine("Created DirectusStore");

            await store.GetProjects();
            Debug.WriteLine("Got all projects");

            AllProjects = store.ProjectsAll;
            OnPropertyChanged("AllProjects");
        }

        public async Task HandleProjectSelectedAsync(Project project)
        {
            store.ProjectSelected = project;
            await store.GetOtherData();
            Debug.WriteLine("Got all other data");

            AllBuildups = store.BuildupsAll;
            List<Mapping> allmappings = store.MappingsAll;
            List<Mapping> projectmappings = store.MappingsProjectUnrelated;

            Debug.WriteLine($"Got {allmappings.Count} all mappings");
            Debug.WriteLine($"Got {projectmappings.Count} project mappings");

            OnPropertyChanged("AllBuildups");
            OnPropertyChanged("Forests");
            OnPropertyChanged("Mappings");
        }

        public void HandleForestSelectionChanged(Forest forest)
        {

            if (forest == null)
                return;
            PlantTrees(forest);
            HandleSideClick();
            OnPropertyChanged("NodeSource");
            ApplyMapping(MappingSelected);
            store.ForestSelected = forest;
        }

        public void HandleMappingSelectionChanged(Mapping mapping)
        {
            MappingSelected = mapping;
            if (CurrentForestItem == null)
                return;
            ApplyMapping(mapping);
        }


        public void HandleNewMapping()
        {
            Window newMappingWindow = new NewMappingView(store);
            newMappingWindow.ShowDialog();
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
            if (nodeItem == null) return;

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

            this.UpdateLiveVisualization();
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

        public void HandleStartCalcLive()
        {
            if (this.server.ConnectedClients == 0)
            {
                Process.Start("https://herzogdemeuron.github.io/calc/");
            }
            Debug.WriteLine(this.server.ConnectedSockets);    
        }

        public void HandleUpdateMapping()
        {
            _ = Task.Run(async () => await this.store.UpdateSelectedMapping());
        }

        private void ApplyMapping(Mapping mapping)
        {

            foreach (NodeViewModel nodeItem in CurrentForestItem.SubNodeItems)
            {
                Tree tree = nodeItem.Host as Tree;
                BranchPainter.ColorBranchesByBranch(tree.SubBranches);

                if (mapping == null)
                    continue;
                mapping.ApplyMappingToTree(tree, AllBuildups);
            };
            HandleBuildupSelectionChanged();
        }

        private void UpdateLiveVisualization()
        {   
            if (this.server == null) return;
            if (this.server.ConnectedClients == 0) return;
            if (CurrentForestItem == null) return;

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

            List<Result> results = Calculator.Calculate(branchesToCalc);
            Debug.WriteLine("GWP calculated");

            _ = Task.Run(async () => await this.server.SendResults(results));
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
