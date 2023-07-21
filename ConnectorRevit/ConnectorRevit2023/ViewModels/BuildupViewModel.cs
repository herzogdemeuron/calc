using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using System.ComponentModel;

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
