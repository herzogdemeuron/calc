using Calc.Core.Color;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Calc.ConnectorRevit.Views
{
    public class BranchViewModel : INotifyPropertyChanged
    {
        public string Name { get => GetName(); }
        private string ProjectName;
        public ObservableCollection<BranchViewModel> SubBranchItems { get; }

        private Branch _branch;
        public Branch Branch
        {
            get { return _branch; }
            set
            {
                if (_branch != value)
                {
                    _branch = value;
                    NotifyPropertyChanged(nameof(Branch));
                    NotifyPropertyChanged(nameof(Buildup));
                }
            }
        }

        public BranchViewModel(Branch branch)
        {
            this.Branch = branch;
            SubBranchItems = new ObservableCollection<BranchViewModel>();

            foreach (var subBranch in branch.SubBranches)
            {
                SubBranchItems.Add(new BranchViewModel(subBranch));
            }
        }

        public BranchViewModel(string projectName, ObservableCollection<BranchViewModel> branchItems)
        {
            ProjectName = projectName;
            this.Branch = new Branch();
            SubBranchItems = branchItems;
        }

        private bool showLabelColor;
        public bool ShowLabelColor
        {
            get => showLabelColor;
            set
            {
                if (showLabelColor != value)
                {
                    showLabelColor = value;
                    NotifyPropertyChanged("LabelColor");
                }
            }
        }
        public Color LabelColor
        {
            get
            {
                if (ShowLabelColor)
                {
                    var hsl = Branch.HslColor;
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
            if(ProjectName != null)
                return ProjectName;
            if (Branch is Tree tree)
                return tree.Name;
            else
                return $"{Branch.Parameter}:{Branch.Value}";
        }

        public void NotifyLabelColorChange()
        {
            NotifyPropertyChanged("LabelColor");
            foreach (var subBranch in SubBranchItems)
            {
                subBranch.NotifyLabelColorChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
