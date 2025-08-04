﻿using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Snapshots;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// Used by the calc project.
    /// Stores Assembly Snapshots, and provides preview of the calculation.
    /// </summary>
    public class CalculationViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel NodeTreeVM;
        public CalcStore Store => NodeTreeVM.Store;
        public NodeModel CurrentNodeItem => NodeTreeVM.SelectedNodeItem ?? NodeTreeVM.CurrentQueryTemplateItem;
        private IGraphNode HostNode => CurrentNodeItem?.Host;
        public double? ProjectArea => Store.ProjectSelected.Area;
        public string Name  // todo: check if needed
        {
            get
            {
                if (HostNode is QueryTemplate qryTemplate)
                    return qryTemplate.Name;
                else
                    return NodeTreeVM.SelectedNodeItem?.Name;
            }
        }
        public string EmptyCalculationText
        {
            get
            {
                if (HasResults || HasErrors)
                {
                    return string.Empty;
                }
                return "- no assembly assigned -";

            }
        }
        public List<AssemblySnapshot> AssemblySnapshots
        {
            get
            {
                if (NodeTreeVM.CurrentQueryTemplateItem?.Host == null) return null;
                var qryTemplate = NodeTreeVM.CurrentQueryTemplateItem.Host as QueryTemplate;
                var queries = qryTemplate.Queries;
                if (queries == null || queries.Count() == 0)
                    return null;
                return queries.SelectMany(q => q.AssemblySnapshots).ToList();
            }
        }
        public bool HasResults => (AssemblySnapshots != null && AssemblySnapshots.Count > 0);

        /// <summary>
        /// Calculation preview on ui, all query snapshots.
        /// </summary>
        public List<QuerySnapshot> QuerySnapshots
        {
            get
            {
                // sum up all results
                var qrySnp = new List<QuerySnapshot>();
                if (AssemblySnapshots == null) return null;
                foreach (var snapshot in AssemblySnapshots)
                {
                    var cal = qrySnp.FirstOrDefault(c => c.QueryName == snapshot.QueryName);
                    if (cal == null)
                    {
                        qrySnp.Add(
                            new QuerySnapshot
                            {
                                QueryName = snapshot.QueryName,
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
                // divide all values by the project area if it is not zero
                foreach (var cal in qrySnp)
                {
                    if (ProjectArea.HasValue && ProjectArea.Value != 0)
                    {
                        cal.Gwp /= ProjectArea.Value;
                        cal.Ge /= ProjectArea.Value;
                    }
                    else
                    {
                        cal.Gwp = 0;
                        cal.Ge = 0;
                    }
                }
                return qrySnp;
            }
        }
        public bool HasErrors => (Errors != null && Errors.Count > 0);
        public bool CanSaveResults => (HasResults && !HasErrors);
        public double ProjectGwp => QuerySnapshots?.Sum(c => c.Gwp) ?? 0;
        public double ProjectGe => QuerySnapshots?.Sum(c => c.Ge) ?? 0;

        /// <summary>
        /// Show the errors for the current selection / whole query template
        /// </summary>
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
                    var qryTemplate = NodeTreeVM.CurrentQueryTemplateItem.Host as QueryTemplate;
                    var branches = qryTemplate.Queries;
                    if (branches == null || branches.Count() == 0)
                        return null;
                    return branches.SelectMany(b => b.ParameterErrors).ToList();
                }
            }
        }

        public CalculationViewModel(NodeTreeViewModel ntVM)
        {
            NodeTreeVM = ntVM;
        }

        /// <summary>
        /// Refreshes the current calculation and errors.
        /// </summary>
        public void RefreshCalculation()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(CurrentNodeItem));
            OnPropertyChanged(nameof(QuerySnapshots));
            OnPropertyChanged(nameof(Errors));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(CanSaveResults));
            OnPropertyChanged(nameof(ProjectGwp));
            OnPropertyChanged(nameof(ProjectGe));
            OnPropertyChanged(nameof(ProjectArea));
            OnPropertyChanged(nameof(EmptyCalculationText));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
