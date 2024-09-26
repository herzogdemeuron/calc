using Calc.MVVM.Helpers.Mediators;
using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Calc.MVVM.Models;

namespace Calc.MVVM.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private readonly NodeModel _node;
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


       

        public Assembly Buildup1
        {
            get => CurrentAssemblies?.Count > 0 ? CurrentAssemblies[0] : null;
            set
            {
                UpdateBuildup(0, value);
                UpdateBuildupSection(false);
                //the combobox selection change due to node item selection change does not
                //belong to assembly selection change broadcast
                MediatorFromVM.Broadcast("BuildupSelectionChanged");
            }
        }

        public Assembly Buildup2
        {
            get => CurrentAssemblies?.Count > 1 ? CurrentAssemblies[1] : null;
            set
            {
                UpdateBuildup(1, value);
                UpdateBuildupSection(false);
                MediatorFromVM.Broadcast("BuildupSelectionChanged");
            }
        }

        public ObservableCollection<Assembly> CurrentAssemblies
        {
            get => _node?.Host is Branch branch ? branch.Assemblies : null;
        }

        private Assembly _activeBuildup;
        public Assembly ActiveBuildup
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
            MediatorFromVM.Register("NodeItemSelectionChanged", _ => UpdateBuildupSection()); // if not, the assembly section sometimes not update,(parent reduced to zero, children remain all enabled), how to solve?
            MediatorFromVM.Register("MappingSelectionChanged", _ => UpdateBuildupSection());
            _node = node;
        }

        public void SetBuildup(bool setMain, Assembly assembly)
        {
            if (setMain)
            {
                Buildup1 = assembly;
            }
            else
            {
                Buildup2 = assembly;
            }
        }

        /// <summary>
        /// Notify the ui change of the assembly properties and the buttons,
        /// broadcast the assembly change to other viewmodels
        /// </summary>
        public void UpdateBuildupSection(bool setFirstBuildupActive = true)
        {
            OnPropertyChanged(nameof(Buildup1));
            OnPropertyChanged(nameof(Buildup2));
            OnPropertyChanged(nameof(CurrentAssemblies));


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
            if (CurrentAssemblies?.Count > 0)
            {
                ActiveBuildup = CurrentAssemblies[0];
            }
        }

        private void UpdateBuildup(int index, Assembly assembly)
        {
            if (_node == null || !(_node.Host is Branch) || _node.Host == null)
                return;
            var branch = _node.Host as Branch;

            var newAssemblies = new List<Assembly>(branch.Assemblies);

            if (index >= newAssemblies.Count)
            {
                newAssemblies.Add(assembly);
            }
            else if (newAssemblies[index] != assembly)
            {
                newAssemblies[index] = assembly;
            }

            branch.SetAssemblies(newAssemblies);
            ActiveBuildup = assembly;

            UpdateBuildupSection(false);
        }

        public void CheckAddBuildup()
        {

            if (_node.Host is Branch branch)
            {
                CanAddFirstBuildup = true;

                var assemblies = branch.Assemblies;
                if (assemblies != null && assemblies.Count > 0)
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

            RemoveEnabled = branch?.Assemblies?.Count > 0;
        }

        public void HandleRemove()
        {
            if (_node == null || _node.Host ==null)
                return;
            var branch = _node.Host as Branch;
            var newAssemblies = new List<Assembly>(branch.Assemblies);
            newAssemblies.RemoveAt(newAssemblies.Count - 1);
            branch.SetAssemblies(newAssemblies);
            var assemblyCount = newAssemblies.Count;
            ActiveBuildup = assemblyCount > 0 ? newAssemblies[assemblyCount - 1] : null;

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
