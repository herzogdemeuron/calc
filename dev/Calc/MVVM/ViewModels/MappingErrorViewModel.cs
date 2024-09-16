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

        private string _buildup1;
        public string Buildup1
        {
            get => _buildup1;
            set
            {
                _buildup1 = value;
                OnPropertyChanged(nameof(Buildup1));
            }
        }

        private string _buildup2;
        public string Buildup2
        {
            get => _buildup2;
            set
            {
                _buildup2 = value;
                OnPropertyChanged(nameof(Buildup2));
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
            MediatorFromVM.Register("BrokenForestChanged", forest => UpdateBrokenNodes((Forest)forest));
        }

        private void UpdateBrokenNodes(Forest forest)
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
                    Buildup1 = "-";
                    Buildup2 = null;
                }
                else
                {
                    var branch = nodeItem.Host as Branch;
                    Buildup1 = branch.Buildups?.Count > 0 ? branch.Buildups[0].ToString() : "-";
                    Buildup2 = branch.Buildups?.Count > 1 ? branch.Buildups[1].ToString() : null;
                }
            }
            else
            {
                Buildup1 = null;
                Buildup2 = null;
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
            Buildup1 = null;
            Buildup2 = null;
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
