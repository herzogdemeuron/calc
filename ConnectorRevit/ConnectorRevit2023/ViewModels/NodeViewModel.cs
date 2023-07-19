using Calc.Core;
using Calc.Core.Color;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        public string Name { get => GetName(); }
        public DirectusStore Store { get; set; }
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
                }
            }
        }

        public NodeViewModel(DirectusStore store, IGraphNode node)
        {
            Host = node;
            Store = store;
            SubNodeItems = new ObservableCollection<NodeViewModel>();

            foreach (var subNode in node.SubBranches)
            {
                SubNodeItems.Add(new NodeViewModel(store, subNode));
            }
        }


        private bool labelColorVisible;
        public bool LabelColorVisible
        {
            get => labelColorVisible;
            set
            {
                if (labelColorVisible != value)
                {
                    labelColorVisible = value;
                    OnPropertyChanged("LabelColorVisible");
                }
            }
        }
        public Color LabelColor
        {
            get
            {
                if (LabelColorVisible)
                {
                    var hsl = Host.HslColor;
                    var rgb = CalcColorConverter.HslToRgb(hsl);
                    return Color.FromArgb(255, rgb.Red, rgb.Green, rgb.Blue);
                }
                else
                {
                    return Color.FromArgb(100, 220, 220, 220);
                }
            }
        }
        public string GetName()
        {
            if (Host is Tree tree)
                return tree.Name;
            else if (Host is Branch branch)
                return $"{branch.Parameter}:{branch.Value}";
            else if (Host is Forest forest)
                return forest.Name;
            else
                return "Unknown";
        }

        public void NotifyLabelColorChange()
        {
            OnPropertyChanged("LabelColor");
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
