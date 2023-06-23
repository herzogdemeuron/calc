using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI.Drivers;
using Calc.ConnectorRevit.EventHandlers;
using Calc.ConnectorRevit.Revit;

namespace Calc.ConnectorRevit.Views
{
    public class ViewModel : INotifyPropertyChanged
    {

        private List<TreeViewItem> _items;
        public List<TreeViewItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private TreeViewItem _selectedItem;
        public TreeViewItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private ExternalEvent _externalVisualizeEvent;
        private ExternalEvent _externalCalculateEvent;
        private ExternalEvent _externalResetVisualizationEvent;

        public ViewModel()
        {
            AddEventHandlers();
            this.Items = CreateTrees();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Visualize()
        {
            _externalVisualizeEvent.Raise();
        }

        public void Calculate()
        {
            try 
            {                 
                _externalCalculateEvent.Raise();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void ResetVisualization()
        {
            _externalResetVisualizationEvent.Raise();
        }

        //public async void GetAllBuildups()
        //{
        //    var driver = new BuildupStorageDriver();
        //    this.AllBuildups = await driver.GetAllBuildupsFromDirectus();
        //}

        public async void GetAllForests()
        {
            this.AllForests = await new ForestStorageDriver().GetAllForestsFromDirectus();
        }

        public async void GetAllMappings()
        {
            this.AllMappings = await new MappingStorageDriver().GetAllMappingsFromDirectus();
        }

        private List<TreeViewItem> CreateTrees()
        {
            Document doc = App.CurrentDoc;
            var root1 = new Root("Type", "Parameter Contains Value", "WAND");
            var root2 = new Root("Type", "Parameter Contains Value", "STB");
            var root3 = new Root("Type", "Parameter Contains Value", "BODN");
            var root4 = new Root("Type", "Parameter Contains Value", "HOB");

            List<Root> roots1 = new List<Root>() { root1, root2 };
            List<Root> roots2 = new List<Root>() { root3, root4 };

            List<List<Root>> rootLists = new List<List<Root>>() { roots1, roots2 };
            var branchConfig = new List<string>() { "Type", "Comments" };

            var _treeViewItems = new List<TreeViewItem>();
            foreach (List<Root> roots in rootLists)
            {
                List<CalcElement> elements = ElementFilter.GetCalcElements(doc, roots);

                //combine values of roots of into a name string
                string name = "";
                foreach (Root root in roots)
                {
                    if (name == "")
                    {
                        name = root.Value;
                    }
                    else
                    {
                        name = name + " - " + root.Value;
                    }
                }

                Tree tree = new Tree(roots)
                {
                    Name = name
                };

                Debug.WriteLine(elements.Count);

                tree.Plant(elements);
                tree.BranchConfig = branchConfig;
                tree.GrowBranches();
                TreeViewItem treeViewItem = new TreeViewItem(tree);
                _treeViewItems.Add(treeViewItem);
            }
            return _treeViewItems;
        }

        private void AddEventHandlers()
        {
            ViewSetEventHandler visualizeEventHandler = new ViewSetEventHandler(this);
            this._externalVisualizeEvent = ExternalEvent.Create(visualizeEventHandler);

            //CalculateEventHandler calculateEventHandler = new CalculateEventHandler(this);
            //this._externalCalculateEvent = ExternalEvent.Create(calculateEventHandler);

            ViewResetEventHandler resetVisualizationEventHandler = new ViewResetEventHandler(this);
            this._externalResetVisualizationEvent = ExternalEvent.Create(resetVisualizationEventHandler);
        }
    }
}
