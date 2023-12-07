using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel nodeTreeVM;

        private readonly int maxBuildups = 2;

        private bool underBuildupLimit = false;
        public bool UnderBuildupLimit
        {
            get => underBuildupLimit;
            set
            {
                underBuildupLimit = value;
                OnPropertyChanged(nameof(UnderBuildupLimit));
            }

        }


        public BuildupViewModel(NodeTreeViewModel ntVM)
        {
            nodeTreeVM = ntVM;
            nodeTreeVM.MaxBuildups = maxBuildups;
        }
        public void UpdateBuildupSection()
        {
            CheckBuildupLimit();
        }

        /// <summary>
        /// determines if the user can add new row of the data grid is enabled
        /// </summary>
        private void CheckBuildupLimit()
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            UnderBuildupLimit = branch.Buildups.Count < maxBuildups;
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

        public void HandleRemove()
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            var newBuildups = new List<Buildup>(branch.Buildups);
            newBuildups.RemoveAt(newBuildups.Count - 1);
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
