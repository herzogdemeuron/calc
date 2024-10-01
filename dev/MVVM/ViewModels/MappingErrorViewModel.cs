using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.MVVM.ViewModels
{

    public class MappingErrorViewModel : INotifyPropertyChanged
    {
        public bool HasBrokenItems => BrokenNodeSource.Count > 0;
        private MappingViewModel mappingVM;
        public ObservableCollection<NodeModel> BrokenNodeSource {  get; private set; }

        private string _assembly1;
        public string Assembly1
        {
            get => _assembly1;
            set
            {
                _assembly1 = value;
                OnPropertyChanged(nameof(Assembly1));
            }
        }

        private string _assembly2;
        public string Assembly2
        {
            get => _assembly2;
            set
            {
                _assembly2 = value;
                OnPropertyChanged(nameof(Assembly2));
            }
        }

        private Forest _brokenForest;
        public Forest BrokenForest
        {
            get => _brokenForest;
            set
            {
                _brokenForest = value;
                OnPropertyChanged(nameof(BrokenForest));
            }
        }

        private Visibility _brokenSectionVisibility = Visibility.Collapsed;
        public Visibility BrokenSectionVisibility
        {
            get => _brokenSectionVisibility;
            set
            {
                _brokenSectionVisibility = value;
                OnPropertyChanged(nameof(BrokenSectionVisibility));
            }
        }

        public MappingErrorViewModel(MappingViewModel mappingViewModel)
        {
            mappingVM = mappingViewModel;
            BrokenNodeSource = new ObservableCollection<NodeModel>();
        }

        internal void UpdateBrokenNodes(Forest forest)
        {
            if(forest.Trees?.Count > 0)
            {
                BrokenForest = forest;
                BrokenNodeSource.Clear();
                foreach (var tree in forest.Trees)
                {
                    BrokenNodeSource.Add(new NodeModel(tree));
                }
            }
            else
            {
                BrokenForest = null;
                BrokenNodeSource.Clear();
            }

            mappingVM.BrokenMappingForest = BrokenForest;
            
            if (BrokenNodeSource.Count > 0)
            {
                BrokenSectionVisibility = Visibility.Visible;
            }

            OnPropertyChanged(nameof(BrokenSectionVisibility));
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public void HandleMappingErrorClicked()
        {
            BrokenSectionVisibility = BrokenSectionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public void HandleBrokenNodeSelectionChanged(NodeModel nodeItem)
        {
            if (nodeItem != null && nodeItem.Host!= null)
            {
                if(nodeItem.Host is Tree)
                {
                    Assembly1 = "-";
                    Assembly2 = null;
                }
                else
                {
                    var branch = nodeItem.Host as Branch;
                    Assembly1 = branch.Assemblies?.Count > 0 ? branch.Assemblies[0].ToString() : "-";
                    Assembly2 = branch.Assemblies?.Count > 1 ? branch.Assemblies[1].ToString() : null;
                }
            }
            else
            {
                Assembly1 = null;
                Assembly2 = null;
            }
        }

        public void RemoveBrokenNode(NodeModel nodeItem)
        {
            if(nodeItem == null)
                return;

            NodeModel nextNode = null;

            if(nodeItem.Host is Tree)
            {
                BrokenNodeSource.Remove(nodeItem);
                BrokenForest.Trees.Remove(nodeItem.Host as Tree);
            }
            else
            {
                nextNode = nodeItem.RemoveFromParent();
            }
            
            if(nextNode != null)
            {
                SelectNode(nextNode);
            }
            else
            {
                DeselectNodes();
            }
            
            // remove empty trees
            foreach (var tree in BrokenNodeSource)
            {
                if (tree.SubNodeItems?.Count == 0)
                {
                    BrokenNodeSource.Remove(tree);
                    BrokenForest.Trees.Remove(tree.Host as Tree);
                    break;
                }
            }

            if (BrokenNodeSource.Count == 0)
            {
                BrokenSectionVisibility = Visibility.Collapsed;
            }

            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
            OnPropertyChanged(nameof(BrokenSectionVisibility));
        }

        public void DeselectNodes()
        {
            Assembly1 = null;
            Assembly2 = null;
            MediatorToView.Broadcast("ViewDeselectBrokenNodesTreeView");
        }

        private void SelectNode(NodeModel node)
        {
            MediatorToView.Broadcast("ViewSelectBrokenNodesTreeView", node);
        }

            public void RemoveAllBrokenNodes()
        {
            BrokenNodeSource.Clear();
            DeselectNodes();
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
