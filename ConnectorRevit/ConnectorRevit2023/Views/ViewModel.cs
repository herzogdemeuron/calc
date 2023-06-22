using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.ConnectorRevit.EventHandlers;
using Calc.ConnectorRevit.Revit;
using System.Linq;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private List<TreeViewItem> items;
        public List<TreeViewItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        private TreeViewItem selectedItem;
        public TreeViewItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public Branch SelectedBranch
        {
            get
            {
                if (SelectedItem == null)
                {
                    return null;
                }
                return SelectedItem.Host;
            }
        }

        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }
        private ExternalEventHandler EventHandler { get; set; }
        private readonly DirectusManager DirectusManager;
        public ViewModel()
        {
            EventHandler = new ExternalEventHandler();
            DirectusManager = new DirectusManager();
        }
        public async Task InitializeAsync()
        {
            Debug.WriteLine("initiating DirectusManager");
            await DirectusManager.Initiate();
            AllBuildups = DirectusManager.AllBuildups;
            AllForests = DirectusManager.AllForests;
            AllMappings = DirectusManager.AllMappings;
            Debug.WriteLine($"AllBuildups.Count: {AllBuildups.Count}");
            Debug.WriteLine($"AllForests.Count: {AllForests.Count}");
            Debug.WriteLine($"AllMappings.Count: {AllMappings.Count}");
            CreateTreeViewItems();
        }



        private void CreateTreeViewItems()
        {
            Forest forest = AllForests.Where(f => f.Name == "RevitTestForest").FirstOrDefault();
            Items = forest.Trees.Select(t =>
            {
                t.Plant(ElementFilter.GetCalcElements(t));
                t.GrowBranches();
                return new TreeViewItem(t);
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
