using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.GraphNodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Calc.MVVM.Models
{

    public class AssemblyModel : INotifyPropertyChanged
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

        private bool _canAddFirstAssembly = false;
        public bool CanAddFirstAssembly
        {
            get => _canAddFirstAssembly;
            set
            {
                _canAddFirstAssembly = value;
                OnPropertyChanged(nameof(CanAddFirstAssembly));
            }
        }

        private bool _canAddSecondAssembly = false;
        public bool CanAddSecondAssembly
        {
            get => _canAddSecondAssembly;
            set
            {
                _canAddSecondAssembly = value;
                OnPropertyChanged(nameof(CanAddSecondAssembly));
            }
        }

        public Assembly Assembly1
        {
            get => CurrentAssemblies?.Count > 0 ? CurrentAssemblies[0] : null;
            set
            {
                UpdateAssembly(0, value);
            }
        }

        public Assembly Assembly2
        {
            get => CurrentAssemblies?.Count > 1 ? CurrentAssemblies[1] : null;
            set
            {
                UpdateAssembly(1, value);
            }
        }

        public ObservableCollection<Assembly> CurrentAssemblies
        {
            get => _node?.Host is Branch branch ? branch.Assemblies : null;
        }

        private Assembly _activeAssembly;
        public Assembly ActiveAssembly
        {
            get => _activeAssembly;
            set
            {
                if (_activeAssembly != value)
                {
                    _activeAssembly = value;
                    OnPropertyChanged(nameof(ActiveAssembly));
                }
            }
        }


        public AssemblyModel(NodeModel node)
        {
            _node = node;
        }

        public void SetAssembly(bool setMain, Assembly assembly)
        {
            if (setMain)
            {
                Assembly1 = assembly;
            }
            else
            {
                Assembly2 = assembly;
            }
        }

        /// <summary>
        /// Notify the ui change of the assembly properties and the buttons,
        /// broadcast the assembly change to other viewmodels
        /// </summary>
        internal void RefreshAssemblySection()
        {
            OnPropertyChanged(nameof(Assembly1));
            OnPropertyChanged(nameof(Assembly2));
            OnPropertyChanged(nameof(CurrentAssemblies));

            CheckInheritEnabled();
            CheckRemoveEnabled();
            CheckAddAssembly();

            //OnPropertyChanged(nameof(CanAddFirstAssembly)); // needed?
            //OnPropertyChanged(nameof(CanAddSecondAssembly)); // needed?
            //OnPropertyChanged(nameof(InheritEnabled)); // needed?
            //OnPropertyChanged(nameof(RemoveEnabled)); // needed?
        }

        public void SetFirstAssemblyToActive()
        {
            if (CurrentAssemblies?.Count > 0)
            {
                ActiveAssembly = CurrentAssemblies[0];
            }
        }

        private void UpdateAssembly(int index, Assembly assembly)
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
            ActiveAssembly = assembly;

            //RefreshAssemblySection(false);
        }

        public void CheckAddAssembly()
        {

            if (_node.Host is Branch branch)
            {
                CanAddFirstAssembly = true;

                var assemblies = branch.Assemblies;
                if (assemblies != null && assemblies.Count > 0)
                {
                    CanAddSecondAssembly = true;
                }
                else
                {
                    CanAddSecondAssembly = false;
                }

            }
            else
            {
                CanAddFirstAssembly = false;
                CanAddSecondAssembly = false;
            }
        }

        public void CheckInheritEnabled()
        {
            if (_node == null || _node.Host is Query)
            {
                InheritEnabled = false;
                return;
            }
            var branch = _node.Host;
            // if branch is branch but not query, then it can inherit
            InheritEnabled = branch is Branch && !(branch is Query);
        }

        public void CheckRemoveEnabled()
        {
            if (_node == null || _node.Host is QueryTemplate)
            {
                RemoveEnabled = false;
                return;
            }
            var branch = _node.Host as Branch;

            RemoveEnabled = branch?.Assemblies?.Count > 0;
        }

        public void HandleRemove()
        {
            if (_node == null || _node.Host == null)
                return;
            var branch = _node.Host as Branch;
            var newAssemblies = new List<Assembly>(branch.Assemblies);
            newAssemblies.RemoveAt(newAssemblies.Count - 1);
            branch.SetAssemblies(newAssemblies);
            var assemblyCount = newAssemblies.Count;
            ActiveAssembly = assemblyCount > 0 ? newAssemblies[assemblyCount - 1] : null;
        }

        public void HandleInherit()
        {
            if (_node == null || _node.Host == null)
                return;
            IGraphNode host = _node.Host;
            if (!(host is Branch branch))
                return;
            branch.InheritMapping();
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
