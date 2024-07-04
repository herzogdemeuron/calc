using Calc.MVVM.Helpers.Mediators;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Calc.MVVM.Models;

namespace Calc.MVVM.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private readonly NodeModel _node;
        private List<Buildup> _buildupsAll => _node.ParentTreeView.AllBuildups;

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
                UpdateBuildupSection(false);
                //the combobox selection change due to node item selection change does not
                //belong to buildup selection change broadcast
                MediatorFromVM.Broadcast("BuildupSelectionChanged");
            }
        }

        public Buildup Buildup2
        {
            get => CurrentBuildups?.Count > 1 ? CurrentBuildups[1] : null;
            set
            {
                UpdateBuildup(1, value);
                UpdateBuildupSection(false);
                MediatorFromVM.Broadcast("BuildupSelectionChanged");
            }
        }

        public ObservableCollection<Buildup> CurrentBuildups
        {
            get => _node?.Host is Branch branch ? branch.Buildups : null;
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


        public BuildupViewModel(NodeModel node)
        {
            MediatorFromVM.Register("NodeItemSelectionChanged", _ => UpdateBuildupSection()); // if not, the buildup section sometimes not update,(parent reduced to zero, children remain all enabled), how to solve?
            MediatorFromVM.Register("MappingSelectionChanged", _ => UpdateBuildupSection());
            _node = node;
        }

        public void SetBuildup(bool setMain, Buildup buildup)
        {
            if (setMain)
            {
                Buildup1 = buildup;
            }
            else
            {
                Buildup2 = buildup;
            }
        }

        /// <summary>
        /// Notify the ui change of the buildup properties and the buttons,
        /// broadcast the buildup change to other viewmodels
        /// </summary>
        public void UpdateBuildupSection(bool setFirstBuildupActive = true)
        {
            OnPropertyChanged(nameof(Buildup1));
            OnPropertyChanged(nameof(Buildup2));
            OnPropertyChanged(nameof(CurrentBuildups));


            CheckInheritEnabled();
            CheckRemoveEnabled();
            CheckAddBuildup();

            OnPropertyChanged(nameof(CanAddFirstBuildup));
            OnPropertyChanged(nameof(CanAddSecondBuildup));
            OnPropertyChanged(nameof(InheritEnabled));
            OnPropertyChanged(nameof(RemoveEnabled));

            if (setFirstBuildupActive)
            {
                SetFirstBuildupToActive();
            }
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
            if (_node == null || !(_node.Host is Branch) || _node.Host == null)
                return;
            var branch = _node.Host as Branch;

            var newBuildups = new List<Buildup>(branch.Buildups);

            if (index >= newBuildups.Count)
            {
                newBuildups.Add(buildup);
            }
            else if (newBuildups[index] != buildup)
            {
                newBuildups[index] = buildup;
            }

            branch.SetBuildups(newBuildups);
            ActiveBuildup = buildup;

            UpdateBuildupSection(false);
        }

        public void CheckAddBuildup()
        {

            if (_node.Host is Branch branch)
            {
                CanAddFirstBuildup = true;

                var buildups = branch.Buildups;
                if (buildups != null && buildups.Count > 0)
                {
                    CanAddSecondBuildup = true;
                }
                else
                {
                    CanAddSecondBuildup = false;
                }

            }
            else
            {
                CanAddFirstBuildup = false;
                CanAddSecondBuildup = false;
            }



        }

        public void CheckInheritEnabled()
        {
            if (_node == null || _node.Host is Tree)
            {
                InheritEnabled = false;
                return;
            }
            var branch = _node.Host;
            // if branch is branch but not tree, then it can inherit
            InheritEnabled = (branch is Branch) && !(branch is Tree);
        }

        public void CheckRemoveEnabled()
        {
            if (_node == null || _node.Host is Forest)
            {
                RemoveEnabled = false;
                return;
            }
            var branch = _node.Host as Branch;

            RemoveEnabled = branch?.Buildups?.Count > 0;
        }

        public void HandleRemove()
        {
            if (_node == null || _node.Host ==null)
                return;
            var branch = _node.Host as Branch;
            var newBuildups = new List<Buildup>(branch.Buildups);
            newBuildups.RemoveAt(newBuildups.Count - 1);
            branch.SetBuildups(newBuildups);
            var buildupCount = newBuildups.Count;
            ActiveBuildup = buildupCount > 0 ? newBuildups[buildupCount - 1] : null;

            MediatorFromVM.Broadcast("BuildupSelectionChanged");
            UpdateBuildupSection();
        }

        public void HandleInherit()
        {
            if (_node == null || _node.Host == null)
                return;
            IGraphNode host = _node.Host;
            if (!(host is Branch branch))
                return;
            branch.InheritMapping();
            MediatorFromVM.Broadcast("BuildupSelectionChanged");
            UpdateBuildupSection();
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
