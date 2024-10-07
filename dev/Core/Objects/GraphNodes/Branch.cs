using Calc.Core.Color;
using Calc.Core.Helpers;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.Mappings;
using Calc.Core.Snapshots;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calc.Core.Objects.GraphNodes
{
    /// <summary>
    /// Used by calc project.
    /// Represents a branch in the query result.
    /// The Query and QueryTemplate are deverived from the branch.
    /// </summary>
    public class Branch : IGraphNode, INotifyPropertyChanged, IColorizable
    {
        public List<CalcElement> Elements { get; set; } = new();
        public double TotalLength
        {
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m).Amount ?? 0
                    ),
                3); 
        }
        public double TotalArea 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m2).Amount ?? 0
                    ),
                3); 
        }
        public double TotalVolume 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m3).Amount ?? 0
                    ),
                3); 
        }
        public List<string> ElementIds => Elements.Select(e => e.Id).ToList();
        public string Parameter { get; set; }
        public string Value { get; set; }
        public List<MappingPath> Path { get => GeneratePath(); }
        public string Method { get; set; }
        private int BranchLevel = 0;
        public List<Branch> SubBranches { get; set; } = new();
        public Branch ParentBranch;
        public Query Query { get; set; }
        public QueryTemplate QueryTemplate { get; set; }
        private ObservableCollection<Assembly> _assemblies = new();
        public ObservableCollection<Assembly> Assemblies  // todo: change to main and sub assembly
        {
            // set _assemblies to empty list if null to avoid null reference exceptions
            get => _assemblies;
            private set
            {
                _assemblies = value;
                OnPropertyChanged(nameof(Assemblies));

                // only for dead end branches:
                if (SubBranches.Count > 0) return;
                CheckParameterErrors(); // check for errors when assemblies are set
                SnapshotMaker.Snap(this); // update the assembly snapshots
            }
        }

        public string ColorIdentifier
        {
            get
            {
                return GetColorIdentifier(Assemblies.ToList());
            }
        }

        private HslColor _hslColor = new(0, 0, 85);
        public HslColor HslColor
        {
            get { return _hslColor; }
            set
            {
                _hslColor = value;
                OnPropertyChanged(nameof(HslColor));
            }
        }

        public bool HasCalculationErrors => (ParameterErrors != null && ParameterErrors.Count > 0); // used in xaml

        private List<ParameterError> _parameterErrors = new();
        public List<ParameterError> ParameterErrors
        {
            get
            {
                if (SubBranches.Count > 0)
                {
                    var errorLists = SubBranches.Select(sb => sb.ParameterErrors).ToList();
                    return ParameterErrorHelper.MergeParameterErrors(errorLists);
                }
                return _parameterErrors;
            }
            set
            {
                _parameterErrors = value;
                OnPropertyChanged(nameof(ParameterErrors));
            }
        }
        private List<AssemblySnapshot> assemblySnapshots = new();
        public List<AssemblySnapshot> AssemblySnapshots
        {
            get
            {
                if (SubBranches.Count > 0)
                {
                    return SubBranches.SelectMany(sb => sb.AssemblySnapshots ?? new List<AssemblySnapshot>()).ToList();
                }
                return assemblySnapshots;
            }
            set
            {
                assemblySnapshots = value;
                OnPropertyChanged(nameof(AssemblySnapshots));
            }
        }

        public Branch()
        {
            Parameter = "No Parameter";
            Method = "No Method";
            Value = "No Value";
            HslColor = HslColor.Default;
        }

        public Branch(List<CalcElement> elements) : this()
        {
            Elements = elements;
        }

        /// <summary>
        /// check if parameters have error for the dead end branch
        /// </summary>
        private void CheckParameterErrors()
        {
            ParameterErrors = new();

            foreach (var element in Elements)
            {
                foreach (var assembly in Assemblies)
                {
                    foreach (var component in assembly.CalculationComponents)
                    {
                        if (component == null) continue;

                        BasicParameter param = element.GetBasicUnitParameter(assembly.AssemblyUnit);

                        if (param.ErrorType != null)
                        {
                            ParameterErrorHelper.AddToErrorList
                                (
                                ParameterErrors,
                                new ParameterError
                                {
                                    ParameterName = param.Name,
                                    Unit = param.Unit,
                                    ErrorType = param.ErrorType,
                                    ElementIds = new() { element.Id }
                                });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates all subbranches from the branch config
        /// </summary>
        internal void CreateBranches(List<string> branchConfig)
        {
            if (branchConfig.Count <= BranchLevel) return;
            var currentParameter = branchConfig[BranchLevel];
            var groupedElements = GroupByParameterValue(currentParameter);
            var methodName = nameof(GroupByParameterValue);
            foreach (KeyValuePair<object, List<CalcElement>> group in groupedElements)
            {
                var branch = new Branch(group.Value)
                {
                    Parameter = currentParameter,
                    Value = group.Key.ToString(),
                    Method = methodName,
                    ParentBranch = this,
                    // set the parent query and query template of the subbranch
                    QueryTemplate = this.QueryTemplate,
                    Query = this.BranchLevel == 0 ? this as Query : this.Query,
                };
                SubBranches.Add(branch);
            }
            if (SubBranches.Count == 0) return;
            for (int index = 0; index < SubBranches.Count; index++)
            {
                var subBranch = SubBranches[index];
                subBranch.BranchLevel = BranchLevel + 1;
                subBranch.CreateBranches(branchConfig);
            }
        }

        /// <summary>
        /// Sets the assembly of the current branch. 
        /// Also sets the assembly of all subbranches to the same value if they have no assembly assigned yet or the assembly is the same.
        /// If it is a dead end branch, calculate the branch with the new assembly.
        /// </summary>
        public void SetAssemblies(List<Assembly> assemblies)
        {
            if (assemblies == null) return;
            var newIdentifier = GetColorIdentifier(assemblies);
            var currentIdentifier = ColorIdentifier;
            if (currentIdentifier == newIdentifier) return;
            Assemblies = new(assemblies);

            OnPropertyChanged(nameof(Assemblies));
            foreach (var subBranch in SubBranches)
            {
                if (subBranch.Assemblies == null || subBranch.ColorIdentifier == currentIdentifier)
                {
                    subBranch.SetAssemblies(assemblies);
                }
            }
        }

        internal void ResetAssemblies()
        {
            Assemblies = new();
            if (SubBranches.Count == 0)
            {
                return;
            }
            foreach (var subBranch in SubBranches)
            {
                subBranch.ResetAssemblies();
            }
        }

        private List<MappingPath> GeneratePath()
        {
            var path = new List<MappingPath>();
            Branch currentBranch = this;
            while (currentBranch.BranchLevel > 0)
            {
                path.Insert(0, new MappingPath
                {
                    Parameter = currentBranch.Parameter,
                    Value = currentBranch.Value
                });

                currentBranch = currentBranch.ParentBranch;
            }
            return path;
        }

        /// <summary>
        /// Uses the assemblies mapping of the parent branch.
        /// </summary>
        public void InheritMapping()
        {
            if (ParentBranch == null)
            {
                return;
            }
            if (ParentBranch.Assemblies != null)
            {
                SetAssemblies(ParentBranch.Assemblies.ToList());
            }
        }

        /// <summary>
        /// Adds a new branch to the current branch using the parameter and value.
        /// </summary>
        /// <returns>The new branch</returns>
        internal Branch AddBranch(string parameter, string value, List<Assembly> assemblies)
        {
            //check if branch already exists
            var existingBranch = SubBranches.FirstOrDefault(sb => sb.Parameter == parameter && sb.Value == value);
            if (existingBranch != null)
            {
                return existingBranch;
            }
            var newBranch = new Branch()
            {
                Parameter = parameter,
                Value = value,
                Assemblies = new ObservableCollection<Assembly>(assemblies),
                ParentBranch = this,
                BranchLevel = this.BranchLevel + 1,
                Query = Query,
                QueryTemplate = QueryTemplate
            };
            SubBranches.Add(newBranch);
            return newBranch;
        }

        /// <summary>
        /// This method returns a flat list of all subbranches. The current branch is included.
        /// </summary>
        internal List<Branch> Flatten()
        {
            var branches = new List<Branch> { this };
            if (SubBranches.Count > 0)
            {
                foreach (var subBranch in SubBranches)
                {
                    branches.AddRange(subBranch.Flatten());
                }
            }
            return branches;
        }

        /// <summary>
        /// Groups calc elements with parameter values they have.
        /// </summary>
        private Dictionary<object, List<CalcElement>> GroupByParameterValue(string parameter)
        {
            var groupedElements = new Dictionary<object, List<CalcElement>>();
            var nullKey = new object(); // Sentinel value for null
            foreach (var element in Elements)
            {
                if (element.Fields.TryGetValue(parameter, out object value))
                {
                    var key = value ?? nullKey;

                    if (groupedElements.ContainsKey(key))
                    {
                        groupedElements[key].Add(element);
                    }
                    else
                    {
                        groupedElements[key] = new List<CalcElement> { element };
                    }
                }
            }
            return groupedElements;
        }

        /// <summary>
        /// The color identifier for current assembly combination.
        /// </summary>
        private string GetColorIdentifier(List<Assembly> assemblies)
        {
            if (assemblies == null)
                return null;
            return string.Join(",", assemblies.Select(b => b?.Id??'-').OrderBy(i => i));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
