using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.Core.Objects;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.ViewModels
{
    public class CalculationViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel NodeTreeVM;
        private IGraphNode HostNode => SelectedNodeItem.Host;
        public NodeViewModel SelectedNodeItem => NodeTreeVM.SelectedNodeItem;
        public bool HasResults => (Results != null && Results.Count > 0);
        public Dictionary<string, decimal> Results
        {
            get
            {
                var calculation = new Dictionary<string, decimal>();
                if (HostNode != null && HostNode is Branch branch)
                {
                    var results = branch.CalculationResults;
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
                }
                return calculation;
            }

        }
        public bool HasErrors => (Errors != null && Errors.Count > 0);

        public List<ParameterError> Errors
        {
            get
            {
                if (HostNode != null && HostNode is Branch branch)
                    return branch.ParameterErrors;
                else
                    return null;
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
            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(Errors));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
