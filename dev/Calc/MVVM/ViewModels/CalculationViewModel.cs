using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Calc.MVVM.Models;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Snapshots;

namespace Calc.MVVM.ViewModels
{
    public class CalculationViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeModel NodeTreeVM;
        public CalcStore Store => NodeTreeVM.Store;
        public NodeModel CurrentNodeItem => NodeTreeVM.SelectedNodeItem ?? NodeTreeVM.CurrentForestItem;
        private IGraphNode HostNode => CurrentNodeItem?.Host;
        public string Name
        {
            get
            {
                if (HostNode is Forest forest)
                    return forest.Name;
                else
                    return NodeTreeVM.SelectedNodeItem?.Name;
            }
        }

        public List<BuildupSnapshot> BuildupSnapshots
        {
            get
            {
                if (HostNode == null) return null;
                if (HostNode is Branch branch)
                {
                    return branch.BuildupSnapshots;
                }
                else
                {
                    var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                    var trees = forest.Trees;
                    if (trees == null || trees.Count() == 0)
                        return null;
                    return trees.SelectMany(t => t.BuildupSnapshots).ToList();
                }

            }
        }
        public bool HasResults => (BuildupSnapshots != null && BuildupSnapshots.Count > 0);
        public List<CategorizedResultModel> CategorizedResults
        {
            get
            {
                var calculation = new List<CategorizedResultModel>();

                if (HostNode == null || BuildupSnapshots == null)
                    return null;

                foreach (var bSnapshot in BuildupSnapshots)
                {
                    var existingResult = calculation.FirstOrDefault(c => c.MaterialFunction == bSnapshot.GroupName);
                    if (existingResult != null)
                    {
                        existingResult.Gwp += Math.Round(bSnapshot.BuildupGwp??0, 3);
                        existingResult.Ge += Math.Round(bSnapshot.Ge??0, 3);
                    }
                    else
                    {
                        calculation.Add(new CategorizedResultModel
                        {
                            MaterialFunction = bSnapshot.GroupName,
                            Gwp = Math.Round(bSnapshot.Gwp, 0),
                            Ge = Math.Round(bSnapshot.Ge, 0)
                        });
                    }
                }

                return calculation;
            }
        }

        public bool HasErrors => (Errors != null && Errors.Count > 0);
        public bool CanSaveResults => (HasResults && !HasErrors && IsFullyCalculated);

        public List<ParameterError> Errors
        {
            get
            {
                if (HostNode == null) return null;
                if (HostNode is Branch branch)
                {
                    return branch.ParameterErrors;
                }
                else
                {
                    var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                    var branches = forest.Trees;
                    if (branches == null || branches.Count() == 0)
                        return null;
                    return branches.SelectMany(b => b.ParameterErrors).ToList();
                }

            }
        }

        private bool IsFullyCalculated
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
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
