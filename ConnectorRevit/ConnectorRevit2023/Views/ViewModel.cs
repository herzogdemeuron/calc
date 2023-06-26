using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI.Drivers;
using Calc.ConnectorRevit.EventHandlers;
using Calc.ConnectorRevit.Revit;
using System.Linq;
using System.Collections.ObjectModel;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private List<Tree> items;
        public List<Tree> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        private Branch selectedBranch;
        public Branch SelectedBranch
        {
            get { return selectedBranch; }
            set
            {
                selectedBranch = value;
                OnPropertyChanged("SelectedBranch");
            }
        }

        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }
        private ExternalEventHandler EventHandler { get; set; }
        private readonly StorageManager DirectusManager;
        public ViewModel()
        {
            EventHandler = new ExternalEventHandler();
            DirectusManager = new StorageManager();
        }
        public async Task InitializeAsync()
        {

            await DirectusManager.Initiate();
            AllBuildups = DirectusManager.AllBuildups;
            OnPropertyChanged("AllBuildups");
            AllForests = DirectusManager.AllForests;
            OnPropertyChanged("AllForests");
            AllMappings = DirectusManager.AllMappings;
            OnPropertyChanged("AllMappings");
            CreateTreeViewItems();
        }

        private void CreateTreeViewItems()
        {
            Forest forest = AllForests.Where(f => f.Name == "RevitTestForest").FirstOrDefault();
            Items = forest.Trees.Select(t=>
            {
                t.Plant(ElementFilter.GetCalcElements(t));
                t.GrowBranches();
                return t;
            }
            ).ToList();
        }
        public void SetView()
        {
            EventHandler.Raise(Visualizer.IsolateAndColor);
        }

        public void ResetView()
        {
            EventHandler.Raise(Visualizer.Reset);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
