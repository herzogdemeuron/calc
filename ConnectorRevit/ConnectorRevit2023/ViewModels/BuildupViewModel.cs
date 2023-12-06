using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel nodeTreeVM;
        private readonly int maxBuildups = 2;
        public bool CanAddBuildup
        {
            get
            {
                if (nodeTreeVM.SelectedNodeItem == null)
                    return false;
                var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
                return branch.Buildups.Count < maxBuildups;
            }
        }

        public void CheckBuildupAddition()
        {
            OnPropertyChanged(nameof(CanAddBuildup));
        }

        public BuildupViewModel(NodeTreeViewModel ntVM)
        {
            nodeTreeVM = ntVM;
            nodeTreeVM.MaxBuildups = maxBuildups;
        }

        public void HandleBuildupSelectionChanged(int index, Buildup buildup)
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            var newBuildups = new List<Buildup>(branch.Buildups);
            if(index + 1 > newBuildups.Count)
            {
                newBuildups.Add(buildup);
            }
            else
            {
                newBuildups[index] = buildup;
            }

            if(newBuildups.Count > maxBuildups)
            {
                newBuildups.RemoveRange(maxBuildups, newBuildups.Count - maxBuildups);
            }
            branch.Buildups = newBuildups;
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
