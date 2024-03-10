using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
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
    public class LayerComponentModel : INotifyPropertyChanged
    {
        public string Name { get => GetNodeName(); }
        public string BranchParameterName { get => GetParameterName(); }
        public bool? BranchParameterIsInstance { get => CheckIfParameterIsInstance(); }
        public bool IsBranch { get => CheckIfBranch(); } // is a branch but not a tree
        public bool IsBrokenNode => BuildupComponent is Branch && (BuildupComponent as Branch).ParentForest == null; // mark as broken if parent forest is null
        public NodeTreeModel ParentTreeView { get; set; }
        public NodeModel ParentNodeItem { get; private set; }
        public ObservableCollection<NodeModel> SubNodeItems { get; }
        public BuildupViewModel NodeBuildupItem { get; set; }

        private BuildupComponent buildupComponent;
        public BuildupComponent BuildupComponent
        {
            get { return buildupComponent; }
            set
            {
                if (buildupComponent != value)
                {
                    buildupComponent = value;
                    OnPropertyChanged(nameof(BuildupComponent));
                    OnPropertyChanged(nameof(LabelColor));
                    OnPropertyChanged(nameof(CategorizedCalculation));
                }
            }
        }

        public Dictionary<string, double> CategorizedCalculation
        {
            get
            {
                var calculation = new Dictionary<string, double>();
                if (BuildupComponent != null && BuildupComponent is Branch branch)
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
                var hsl = BuildupComponent.HslColor;
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
                if (LabelColorVisible || BuildupComponent is Forest)
                {
                    return IdentifierColor;
                }
                else
                {
                    return Color.FromArgb(100, 219, 219, 219);
                }
            }
        }

        public BuildupComponentModel(IGraphNode node, NodeTreeModel parentTreeView = null, NodeModel parentNodeItem = null)
        {
            BuildupComponent = node;
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

        public string GetParameterName()
        {
            if (BuildupComponent is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item2;
            else
                return null;
        }

        public bool? CheckIfParameterIsInstance()
        {
            if (BuildupComponent is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item1;
            else
                return null;
        }

        public bool CheckIfBranch()
        {
            return BuildupComponent is Branch && !(BuildupComponent is Tree);
        }
        public void NotifyNodePropertyChange()
        {
            OnPropertyChanged(nameof(IdentifierColor));
            OnPropertyChanged(nameof(UnderlineVisibility));
            OnPropertyChanged(nameof(LabelColor));
            OnPropertyChanged(nameof(LabelColorVisible));

            OnPropertyChanged(nameof(BuildupComponent));
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
