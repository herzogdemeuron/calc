using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private readonly NodeTreeViewModel nodeTreeVM;

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

        private bool _canAddFirstBuildup = false;
        public bool CanAddFirstBuildup
        {
            get => _canAddFirstBuildup;
            set
            {
                _canAddFirstBuildup = value;
                OnPropertyChanged(nameof(CanAddFirstBuildup));
            }
        }

        private bool _canAddSecondBuildup = false;
        public bool CanAddSecondBuildup
        {
            get => _canAddSecondBuildup;
            set
            {
                _canAddSecondBuildup = value;
                OnPropertyChanged(nameof(CanAddSecondBuildup));
            }
        }

        public Buildup Buildup1
        {
            get => CurrentBuildups?.Count > 0 ? CurrentBuildups[0] : null;
            set
            {
                UpdateBuildup(0, value);
                UpdateBuildupsUI();
            }
        }

        public Buildup Buildup2
        {
            get => CurrentBuildups?.Count > 1 ? CurrentBuildups[1] : null;
            set
            {
                UpdateBuildup(1, value);
                UpdateBuildupsUI();
            }
        }

        public ObservableCollection<Buildup> CurrentBuildups
        {
            get => nodeTreeVM.SelectedNodeItem?.Host is Branch branch ? branch.Buildups : null;
        }

        private Buildup _activeBuildup;
        public Buildup ActiveBuildup
        {
            get => _activeBuildup;
            set
            {
                if (_activeBuildup != value)
                {
                    _activeBuildup = value;
                    OnPropertyChanged(nameof(ActiveBuildup));
                }
            }
        }


        public BuildupViewModel(NodeTreeViewModel ntVM)
        {
            nodeTreeVM = ntVM;
        }
        public void UpdateBuildupsUI()
        {
            //CheckBuildupLimit();
            OnPropertyChanged(nameof(Buildup1));
            OnPropertyChanged(nameof(Buildup2));
            OnPropertyChanged(nameof(CurrentBuildups));

            CheckInheritEnabled();
            CheckRemoveEnabled();
            CheckAddBuildup();
        }

        public void SetFirstBuildupToActive()
        {
            if (CurrentBuildups?.Count > 0)
            {
                ActiveBuildup = CurrentBuildups[0];
            }
        }

        private void UpdateBuildup(int index, Buildup buildup)
        {
            if (nodeTreeVM.SelectedNodeItem == null || !(nodeTreeVM.SelectedNodeItem.Host is Branch))
                return;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            var newBuildups = new List<Buildup>(branch.Buildups);
            if (index >= newBuildups.Count)
            {
                newBuildups.Add(buildup);
            }
            else
            {
                newBuildups[index] = buildup;
            }
            branch.SetBuildups(newBuildups);
            ActiveBuildup = buildup;
            Mediator.Broadcast("BuildupSelectionChanged");
        }

        public void CheckAddBuildup()
        {
            if (nodeTreeVM.SelectedNodeItem == null || !(nodeTreeVM.SelectedNodeItem.Host is Branch))
            {
                CanAddFirstBuildup = false;
                CanAddSecondBuildup = false;
                return;
            }
            CanAddFirstBuildup = true;
            var branch = nodeTreeVM.SelectedNodeItem.Host as Branch;
            CanAddSecondBuildup = branch.Buildups?.Count > 0;
        }

        public void CheckInheritEnabled()
        {
            if (nodeTreeVM.SelectedNodeItem == null || nodeTreeVM.SelectedNodeItem.Host is Tree)
            {
                InheritEnabled = false;
                return;
            }
            var branch = nodeTreeVM.SelectedNodeItem.Host;
            // if branch is branch but not tree, then it can inherit
            InheritEnabled = (branch is Branch) && !(branch is Tree);
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
            var buildupCount = newBuildups.Count;
            ActiveBuildup = buildupCount > 0 ? newBuildups[buildupCount - 1] : null;
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
