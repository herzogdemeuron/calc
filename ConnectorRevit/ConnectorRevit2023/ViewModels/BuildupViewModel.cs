using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Services;
using System.Diagnostics;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private NodeTreeViewModel nodeTreeVM;
        public BuildupViewModel(NodeTreeViewModel ntVM)
        {
            nodeTreeVM = ntVM;
        }

        public void HandleBuildupSelectionChanged()
        {
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        public void HandleInherit()
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            IGraphNode host = nodeTreeVM.SelectedNodeItem.Host;
            if (!(host is Branch branch))
                return;
            branch.InheritMapping();
            Mediator.Broadcast("BuildupSelectionChanged");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
