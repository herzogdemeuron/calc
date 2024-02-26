using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using Calc.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Calc.MVVM.Models
{
    public enum NodeError
    {
        DoorOrWindowWithoutArea
    }
    public class NodeModel : INotifyPropertyChanged
    {
        public string Name { get => GetNodeName(); }
        public string BranchParameterName { get => GetParameterName(); }
        public bool? BranchParameterIsInstance { get => CheckIfParameterIsInstance(); }
        public bool IsBranch { get => CheckIfBranch(); } // is a branch but not a tree
        public bool IsBrokenNode => Host is Branch && (Host as Branch).ParentForest == null; // mark as broken if parent forest is null
        public NodeTreeModel ParentTreeView { get; set; }
        public NodeModel ParentNodeItem { get; private set; }
        public ObservableCollection<NodeModel> SubNodeItems { get; }
        public BuildupViewModel NodeBuildupItem { get; set; }

        private IGraphNode host;
        public IGraphNode Host
        {
            get { return host; }
            set
            {
                if (host != value)
                {
                    host = value;
                    OnPropertyChanged(nameof(Host));
                    OnPropertyChanged(nameof(LabelColor));
                    OnPropertyChanged(nameof(CategorizedCalculation));
                }
            }
        }

        public Dictionary<string, decimal> CategorizedCalculation
        {
            get
            {
                var calculation = new Dictionary<string, decimal>();
                if (Host != null && Host is Branch branch)
                {
                    var results = branch.CalculationResults;
                    foreach (var result in results)
                    {
                        if (calculation.ContainsKey(result.GroupName))
                        {
                            calculation[result.GroupName] += Math.Round(result.Gwp, 3);
                        }
                        else
                        {
                            calculation.Add(result.GroupName, Math.Round(result.Gwp, 3));
                        }
                    }
                }
                return calculation;
            }
        }

        private bool _labelColorVisible;
        public bool LabelColorVisible
        {
            get => _labelColorVisible;
            set
            {
                if (_labelColorVisible != value)
                {
                    _labelColorVisible = value;
                    OnPropertyChanged(nameof(LabelColorVisible));
                }
            }
        }

        public Color IdentifierColor
        {
            get
            {
                var hsl = Host.HslColor;
                var rgb = CalcColorConverter.HslToRgb(hsl);
                return Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
            }
        }

        public Visibility UnderlineVisibility
        {
            get
            {
                return ParentTreeView?.BranchesSwitch == false ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Color LabelColor
        {
            get
            {
                if (LabelColorVisible || Host is Forest)
                {
                    return IdentifierColor;
                }
                else
                {
                    return Color.FromArgb(100, 219, 219, 219);
                }
            }
        }

        public NodeModel(IGraphNode node, NodeTreeModel parentTreeView = null, NodeModel parentNodeItem = null)
        {
            Host = node;
            ParentTreeView = parentTreeView;
            SubNodeItems = new ObservableCollection<NodeModel>();
            NodeBuildupItem = new BuildupViewModel(this);

            foreach (var subNode in node.SubBranches)
            {
                SubNodeItems.Add(new NodeModel(subNode, parentTreeView, this));
            }
            ParentNodeItem = parentNodeItem;
        }

        /// <summary>
        /// remove the branch node from its parent node,
        /// returns the next node to select.
        /// </summary>
        public NodeModel RemoveFromParent()
        {
            if (ParentNodeItem == null || IsBranch == false)
                return null;

            var index = ParentNodeItem.SubNodeItems.IndexOf(this);

            ParentNodeItem.SubNodeItems.RemoveAt(index);
            ParentNodeItem.Host.SubBranches.RemoveAt(index);

            if (index < ParentNodeItem.SubNodeItems.Count)
            {
                return ParentNodeItem.SubNodeItems[index];
            }
            else
            {
                return null;
            }
        }

        public string GetNodeName()
        {
            if (Host is Tree tree)
                return tree.Name;
            else if (Host is Branch branch)
                return branch.Value;
            else if (Host is Forest forest)
                return forest.Name;
            else
                return "Unknown";
        }

        public string GetParameterName()
        {
            if (Host is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item2;
            else
                return null;
        }

        public bool? CheckIfParameterIsInstance()
        {
            if (Host is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item1;
            else
                return null;
        }

        public bool CheckIfBranch()
        {
            return Host is Branch && !(Host is Tree);
        }
        public void NotifyNodePropertyChange()
        {
            OnPropertyChanged(nameof(IdentifierColor));
            OnPropertyChanged(nameof(UnderlineVisibility));
            OnPropertyChanged(nameof(LabelColor));
            OnPropertyChanged(nameof(LabelColorVisible));

            OnPropertyChanged(nameof(Host));
            OnPropertyChanged(nameof(CategorizedCalculation));

            foreach (var subBranch in SubNodeItems)
            {
                subBranch.NotifyNodePropertyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
