using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Calc.ConnectorRevit.Views
{
    public class BranchViewModel : INotifyPropertyChanged
    {
        public Branch Branch { get;}
        public ObservableCollection<BranchViewModel> SubBranchItems { get; }
        public BranchViewModel(Branch branch)
        {
            this.Branch = branch;
            SubBranchItems = new ObservableCollection<BranchViewModel>();
            foreach (var subBranch in branch.SubBranches)
            {
                SubBranchItems.Add(new BranchViewModel(subBranch));
            }
        }

        public Buildup Buildup
        {
            get => Branch.Buildup;
            set
            {
                if (Branch.Buildup != value)
                {
                    Branch.Buildup = value;
                    NotifyPropertyChanged("Buildup");
                }
            }
        }

        public Color Color
        {
            get
            {
                if (DisplayColor)
                {
                    var hsl = Branch.HslColor;
                    var rgb = Core.Color.ColorConverter.HslToRgb(hsl);
                    return Color.FromArgb(150, rgb.Red, rgb.Green, rgb.Blue);
                }
                else
                {
                    //light gray color
                    return Color.FromArgb(100, 220, 220, 220);
                }
            }
        }

        private bool displayColor;
        public bool DisplayColor
        {
            get => displayColor;
            set
            {
                if (displayColor != value)
                {
                    displayColor = value;
                    //NotifyPropertyChanged("DisplayColor");
                    NotifyPropertyChanged("Color");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateBuildups()
        {
            Buildup = Branch.Buildup;
            foreach (BranchViewModel subBranch in SubBranchItems)
            {
                subBranch.UpdateBuildups();
            }
            
        }
    }
}
