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
        public string Name { get => GetName(); }
        public string ShortName { get => GetName(false); }
        public DirectusStore Store { get; set; }
        public NodeTreeViewModel ParentTreeView { get; set; }
        public ObservableCollection<NodeViewModel> SubNodeItems { get; }

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
        public string ElementsCount
        {
            get
            {
                if (Host == null)
                    return "Choose a forest";
                else
                    return $"({ Host.Elements.Count})";
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


        public NodeViewModel(DirectusStore store, IGraphNode node, NodeTreeViewModel parentTreeView)
        {
            Host = node;
            Store = store;
            ParentTreeView = parentTreeView;
            SubNodeItems = new ObservableCollection<NodeViewModel>();
            Mediator.Register("BuildupSelectionChanged", _ => NotifyHostChanged());
            foreach (var subNode in node.SubBranches)
            {
                SubNodeItems.Add(new NodeViewModel(store, subNode, parentTreeView));
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
                return ParentTreeView.BranchesSwitch == false? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Color LabelColor
        {
            get
            {
                if (LabelColorVisible)
                {
                    return IdentifierColor;
                }
                else
                {
                    return Color.FromArgb(100, 220, 220, 220);
                }
            }
        }
        public string GetName(bool showType = true)
        {
            if (Host is Tree tree)
                return tree.Name;
            else if (Host is Branch branch)
                return showType?$"[{branch.Parameter}] {branch.Value}": branch.Value;
            else if (Host is Forest forest)
                return forest.Name;
            else
                return "Unknown";
        }

        private void NotifyHostChanged()
        {
            OnPropertyChanged(nameof(Host));
            OnPropertyChanged(nameof(CategorizedCalculation));
        }

        public void NotifyLabelColorChange()
        {
            OnPropertyChanged(nameof(IdentifierColor));
            OnPropertyChanged(nameof(UnderlineVisibility));
            OnPropertyChanged(nameof(LabelColor));
            OnPropertyChanged(nameof(LabelColorVisible));
            foreach (var subBranch in SubNodeItems)
            {
                subBranch.NotifyLabelColorChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
