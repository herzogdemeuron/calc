﻿using Calc.ConnectorRevit.Helpers;
using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.Core.Objects.GraphNodes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.ConnectorRevit.ViewModels
{

    public class MappingErrorViewModel : INotifyPropertyChanged
    {
        public bool HasBrokenItems => BrokenNodeSource.Count > 0;
        public ObservableCollection<NodeViewModel> BrokenNodeSource {  get; private set; }

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

        public MappingErrorViewModel( )
        {
            BrokenNodeSource = new ObservableCollection<NodeViewModel>();
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
                    BrokenNodeSource.Add(new NodeViewModel(tree));
                }
            }
            else
            {
                BrokenForest = null;
                BrokenNodeSource.Clear();
            }
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public void HandleMappingErrorClicked()
        {
            BrokenSectionVisibility = BrokenSectionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public void HandleBrokenNodeSelectionChanged(NodeViewModel nodeItem)
        {
            if (nodeItem != null && nodeItem.Host!= null)
            {
                var branch = nodeItem.Host as Branch;
                Buildup1 = branch.Buildups?.Count > 0 ? branch.Buildups[0].ToString() : null;
                Buildup2 = branch.Buildups?.Count > 1 ? branch.Buildups[1].ToString() : null;
            }
            else
            {
                Buildup1 = null;
                Buildup2 = null;
            }
        }

        public void RemoveBrokenNode(NodeViewModel nodeItem)
        {
            if(nodeItem == null)
                return;
            foreach (var tree in BrokenNodeSource)
            {
                if(tree == nodeItem)
                {
                    BrokenNodeSource.Remove(tree);
                    DeselectNodes();
                    break;
                }
                else
                {
                    var result = tree.RemoveSubNode(nodeItem);
                    var removed = result.Item1;
                    var nextNode = result.Item2;
                    if (removed)
                    {
                        SelectNode(nextNode);
                        break;
                    }
                }
            }
            // remove empty trees
            foreach (var tree in BrokenNodeSource)
            {
                if (tree.SubNodeItems?.Count == 0)
                {
                    BrokenNodeSource.Remove(tree);
                    break;
                }
            }
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public void DeselectNodes()
        {
            Buildup1 = null;
            Buildup2 = null;
            MediatorToView.Broadcast("ViewDeselectBrokenNodesTreeView");
        }

        private void SelectNode(NodeViewModel node)
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
