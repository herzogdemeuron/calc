using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Calc.Core.Color;

namespace Calc.ConnectorRevit.Views
{
    public class BranchViewModel : INotifyPropertyChanged
    {
        public Branch Branch { get;}
        public string Name { get => GetName();}
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


        public Color Color
        {
            get
            {
                if (DisplayColor)
                {
                    var hsl = Branch.HslColor;
                    var rgb = CalcColorConverter.HslToRgb(hsl);
                    return Color.FromArgb(100, rgb.Red, rgb.Green, rgb.Blue);
                }
                else
                {
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

        public string GetName()
        {
            if (Branch is Tree tree)
                return tree.Name;
            else
                return $"{Branch.Parameter}:{Branch.Value}";
        }

        public void UpdateColor()
        {
            NotifyPropertyChanged("Color");
            foreach (var subBranch in SubBranchItems)
            {
                subBranch.UpdateColor();
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
