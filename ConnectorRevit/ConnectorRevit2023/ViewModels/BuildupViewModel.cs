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

        private bool _inheritEnabled = false;
        public bool InheritEnabled
        {
            get => _inheritEnabled;
            set
            {
                _inheritEnabled = value;
                OnPropertyChanged(nameof(InheritEnabled));
            }
        }
        private bool _removeEnabled = false;
        public bool RemoveEnabled
        {
            get => _removeEnabled;
            set
            {
                _removeEnabled = value;
                OnPropertyChanged(nameof(RemoveEnabled));
            }
        }

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

        private Buildup _selectedBuildup;
        public Buildup SelectedBuildup
        {
            get => _selectedBuildup;
            set
            {
                if (_selectedBuildup != value)
                {
                    _selectedBuildup = value;
                    OnPropertyChanged(nameof(SelectedBuildup));
                }
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
            CheckInheritEnabled();
            CheckRemoveEnabled();
        }

        /// <summary>
        /// determines if the user can add new row of the data grid is enabled
        /// </summary>
        private void CheckBuildupLimit()
        {
            if (nodeTreeVM.SelectedNodeItem == null || !(nodeTreeVM.SelectedNodeItem.Host is Branch))
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            UnderBuildupLimit = branch.Buildups.Count < maxBuildups;
        }

        public void HandleBuildupSelectionChanged(int index, Buildup buildup)
        {
            if (nodeTreeVM.SelectedNodeItem == null || !(nodeTreeVM.SelectedNodeItem.Host is Branch))
                return;

            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;

            var newBuildups = new List<Buildup>(branch.Buildups);
            if(index + 1 > newBuildups.Count)
            {
                newBuildups.Add(buildup);
            }
            else if(newBuildups[index] == buildup)
            { 
               return;
            }
            else
            {
                newBuildups[index] = buildup;
            }

            if(newBuildups.Count > maxBuildups)
            {
                newBuildups.RemoveRange(maxBuildups, newBuildups.Count - maxBuildups);
            }
            branch.SetBuildups(newBuildups);
            SelectedBuildup = branch.Buildups[index];
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        public void CheckInheritEnabled()
        {
            if (nodeTreeVM.SelectedNodeItem == null || nodeTreeVM.SelectedNodeItem.Host is Tree)
            {
                InheritEnabled = false;
                return;
            }
            InheritEnabled = true;
        }

        public void CheckRemoveEnabled()
        {
            if (nodeTreeVM.SelectedNodeItem == null || nodeTreeVM.SelectedNodeItem.Host is Forest)
            {
                RemoveEnabled = false;
                return;
            }
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;

            RemoveEnabled = branch.Buildups?.Count > 0;
        }

        public void HandleRemove()
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            var newBuildups = new List<Buildup>(branch.Buildups);
            newBuildups.RemoveAt(newBuildups.Count - 1);
            branch.SetBuildups(newBuildups);
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
