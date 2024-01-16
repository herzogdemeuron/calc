using Calc.ConnectorRevit.Helpers.Mediators;
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
        public bool HasResults => (Results != null && Results.Count > 0);
        public Dictionary<string, decimal> Results
        {
            get
            {
                var calculation = new Dictionary<string, decimal>();
                var results = new List<Result>();
                if (HostNode == null)
                    return null;
                if (HostNode is Branch branch)
                {
                    results = branch.CalculationResults;
                }
                else
                {
                    var forest = NodeTreeVM.CurrentForestItem.Host as Forest;
                    var branches = forest.Trees.SelectMany(tree => tree.Flatten());
                    if (branches == null || branches.Count() == 0) 
                        return calculation;
                    results = branches.SelectMany(b => b.CalculationResults).ToList();
                }

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
            OnPropertyChanged(nameof(Results));
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
