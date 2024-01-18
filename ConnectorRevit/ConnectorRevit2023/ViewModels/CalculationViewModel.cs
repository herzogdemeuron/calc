using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.ConnectorRevit.ViewModels
{
    public class CalculationViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel NodeTreeVM;
        public DirectusStore Store => NodeTreeVM.Store;
        public NodeViewModel CurrentNodeItem => NodeTreeVM.SelectedNodeItem ?? NodeTreeVM.CurrentForestItem;
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

        public List<Result> Results
        {
            get
            {
                if (HostNode == null) return null;
                if (HostNode is Branch branch)
                {
                    return branch.CalculationResults;
                }
                else
                {
                    var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                    var branches = forest.Trees.SelectMany(tree => tree.Flatten());
                    if (branches == null || branches.Count() == 0)
                        return null;
                    return branches.SelectMany(b => b.CalculationResults).ToList();
                }

            }
        }
        public bool HasResults => (Results != null && Results.Count > 0);
        public List<CategorizedResultViewModel> CategorizedResults
        {
            get
            {
                var calculation = new List<CategorizedResultViewModel>();

                if (HostNode == null || Results == null)
                    return null;

                foreach (var result in Results)
                {
                    var existingResult = calculation.FirstOrDefault(c => c.GroupName == result.GroupName);
                    if (existingResult != null)
                    {
                        existingResult.Gwp += Math.Round(result.Gwp, 3);
                        existingResult.Ge += Math.Round(result.Ge, 3);
                    }
                    else
                    {
                        calculation.Add(new CategorizedResultViewModel
                        {
                            GroupName = result.GroupName,
                            Gwp = Math.Round(result.Gwp, 3),
                            Ge = Math.Round(result.Ge, 3)
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

        public CalculationViewModel(NodeTreeViewModel ntVM)
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
