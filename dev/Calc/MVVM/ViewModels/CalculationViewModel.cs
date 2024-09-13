using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Snapshots;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.ViewModels
{
    public class CalculationViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeModel NodeTreeVM;
        public CalcStore Store => NodeTreeVM.Store;
        private NodeModel CurrentNodeItem => NodeTreeVM.SelectedNodeItem ?? NodeTreeVM.CurrentForestItem;
        private IGraphNode HostNode => CurrentNodeItem?.Host;
        private double ProjectArea => Store.ProjectSelected.Area;
        public string Name  // used anywhere?
        {
            get
            {
                if (HostNode is Forest forest)
                    return forest.Name;
                else
                    return NodeTreeVM.SelectedNodeItem?.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<BuildupSnapshot> BuildupSnapshots
        {
            get
            {
                if (NodeTreeVM.CurrentForestItem?.Host == null) return null;
                var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                var trees = forest.Trees;
                if (trees == null || trees.Count() == 0)
                    return null;
                return trees.SelectMany(t => t.BuildupSnapshots).ToList();
            }
        }

        public bool HasResults => (BuildupSnapshots != null && BuildupSnapshots.Count > 0);

        /// <summary>
        /// calculation for each element group
        /// </summary>
        public List<CategorizedResultModel> CategorizedResults
        {
            get
            {
                var calculation = new List<CategorizedResultModel>();
                if (BuildupSnapshots == null)
                    return null;

                foreach (var snapshot in BuildupSnapshots)
                {
                    var cal = calculation.FirstOrDefault(c => c.Group == snapshot.ElementGroup);
                    if (cal == null)
                    {
                        calculation.Add(
                            new CategorizedResultModel
                            {
                                Group = snapshot.ElementGroup,
                                Gwp = snapshot.TotalGwp.Value,
                                Ge = snapshot.TotalGe.Value
                            });
                    }
                    else
                    {
                        cal.Gwp += snapshot.TotalGwp.Value;
                        cal.Ge += snapshot.TotalGe.Value;
                    }
                }

                // post process: divide all values by the project area if it is not zero
                foreach (var cal in calculation)
                {
                    if (ProjectArea != 0)
                    {
                        cal.Gwp /= ProjectArea;
                        cal.Ge /= ProjectArea;
                    }
                    else
                    {
                        cal.Gwp = 0;
                        cal.Ge = 0;
                    }
                }

                return calculation;
            }
        }

        public bool HasErrors => (Errors != null && Errors.Count > 0);
        public bool CanSaveResults => (HasResults && !HasErrors);
        public double ProjectGwp => CategorizedResults?.Sum(c => c.Gwp) ?? 0;
        public double ProjectGe => CategorizedResults?.Sum(c => c.Ge) ?? 0;

        public List<ParameterError> Errors
        {
            get
            {
                if (NodeTreeVM.CurrentForestItem?.Host == null) return null;
                var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                var branches = forest.Trees;
                if (branches == null || branches.Count() == 0)
                    return null;
                return branches.SelectMany(b => b.ParameterErrors).ToList();
            }
        }

        private bool IsFullyCalculated // deprecated
        {
            get
            {
                if (HostNode == null) return false;
                if (HostNode is Branch branch)
                {
                    return branch.IsFullyCalculated;
                }
                else
                {
                    var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                    var branches = forest.Trees;
                    if (branches == null || branches.Count() == 0)
                        return true;
                    return branches.All(b => b.IsFullyCalculated);
                }
            }
        }

        public CalculationViewModel(NodeTreeModel ntVM)
        {
            NodeTreeVM = ntVM;
            MediatorFromVM.Register("BuildupSelectionChanged", _ => NotifyCalculationChanged());
            MediatorFromVM.Register("NodeItemSelectionChanged", _ => NotifyCalculationChanged());
            MediatorFromVM.Register("MappingSelectionChanged", _ => NotifyCalculationChanged());
        }

        private void NotifyCalculationChanged()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(CurrentNodeItem));
            OnPropertyChanged(nameof(CategorizedResults));
            OnPropertyChanged(nameof(Errors));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(CanSaveResults));
            OnPropertyChanged(nameof(ProjectGwp));
            OnPropertyChanged(nameof(ProjectGe));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
