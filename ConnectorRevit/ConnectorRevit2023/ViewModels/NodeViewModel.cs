using Calc.ConnectorRevit.Helpers;
using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Calc.ConnectorRevit.ViewModels
{
    public enum NodeError
    {
        DoorOrWindowWithoutArea
    }
    public class NodeViewModel : INotifyPropertyChanged
    {
        public string Name { get => GetNodeName(); }
        public string BranchParameterName { get => GetParameterName(); }
        public bool? BranchParameterIsInstance { get => CheckIfParameterIsInstance(); }
        public bool IsBranch { get => CheckIfBranch(); }
        public NodeTreeViewModel ParentTreeView { get; set; }
        public ObservableCollection<NodeViewModel> SubNodeItems { get; }
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
                return ParentTreeView?.BranchesSwitch == false? Visibility.Visible : Visibility.Collapsed;
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

        public NodeViewModel(IGraphNode node, NodeTreeViewModel parentTreeView = null)
        {
            Host = node;
            ParentTreeView = parentTreeView;
            SubNodeItems = new ObservableCollection<NodeViewModel>();
            NodeBuildupItem = new BuildupViewModel(this);

            foreach (var subNode in node.SubBranches)
            {
                SubNodeItems.Add(new NodeViewModel(subNode, parentTreeView));
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
            return (Host is Branch) && !(Host is Tree);
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
