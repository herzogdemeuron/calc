using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Views
{
    public class BranchViewModel : INotifyPropertyChanged
    {
        public Branch Branch { get;}
        public ObservableCollection<BranchViewModel> SubBranches { get; }
        public BranchViewModel(Branch branch)
        {
            this.Branch = branch;
            SubBranches = new ObservableCollection<BranchViewModel>();
            foreach (var subBranch in branch.SubBranches)
            {
                SubBranches.Add(new BranchViewModel(subBranch));
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateBuildups()
        {
            Buildup = Branch.Buildup;
            foreach (BranchViewModel subBranch in SubBranches)
            {
                subBranch.UpdateBuildups();
            }
            
        }
    }
}
