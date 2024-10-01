using Calc.Core.Color;
using Calc.Core.Helpers;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.Core.Objects.Mappings;
using Calc.Core.Snapshots;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calc.Core.Objects.GraphNodes
{
    public class Branch : IGraphNode, INotifyPropertyChanged, IColorizable
    {
        [JsonIgnore]
        public List<CalcElement> Elements { get; set; } = new();
        [JsonIgnore]
        public double TotalLength
        {
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m).Amount ?? 0
                    ),
                3); 
        }
        [JsonIgnore]
        public double TotalArea 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m2).Amount ?? 0
                    ),
                3); 
        }
        [JsonIgnore]
        public double TotalVolume 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m3).Amount ?? 0
                    ),
                3); 
        }

        [JsonIgnore]
        public List<string> ElementIds => Elements.Select(e => e.Id).ToList();
        [JsonIgnore]
        public string Parameter { get; set; }
        [JsonIgnore]
        public string Value { get; set; }
        [JsonIgnore]
        public List<MappingPath> Path { get => GeneratePath(); }
        [JsonIgnore]
        public string Method { get; set; }
        [JsonIgnore]
        public int BranchLevel = 0;
        [JsonIgnore]
        public List<Branch> SubBranches { get; set; } = new();
        [JsonIgnore]
        public Branch ParentBranch;
        [JsonIgnore]
        public Tree ParentTree { get; set; }
        [JsonIgnore]
        public Forest ParentForest { get; set; }
        [JsonIgnore]
        private ObservableCollection<Assembly> _assemblies = new();
        [JsonIgnore]
        public ObservableCollection<Assembly> Assemblies  // change to main and sub assembly
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
        [JsonIgnore]
        public string ColorIdentifier
        {
            get
            {
                return GetColorIdentifier(Assemblies.ToList());
            }
        }
        private HslColor _hslColor = new(0, 0, 85);
        [JsonIgnore]
        public HslColor HslColor
        {
            get { return _hslColor; }
            set
            {
                _hslColor = value;
                OnPropertyChanged(nameof(HslColor));
            }
        }

        [JsonIgnore]
        public bool HasCalculationErrors => (ParameterErrors != null && ParameterErrors.Count > 0);

        [JsonIgnore]
        public bool IsFullyCalculated // deprecatd
        {
            get
            {
                if (SubBranches.Count > 0)
                {
                    return SubBranches.All(sb => sb.IsFullyCalculated);
                }
                return HasCalculationResults || ElementIds.Count == 0;
            }
        }

        private List<ParameterError> _parameterErrors = new();
        [JsonIgnore]
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

        [JsonIgnore]
        public bool HasCalculationResults => (AssemblySnapshots != null && AssemblySnapshots.Count > 0);
        private List<AssemblySnapshot> assemblySnapshots = new();
        [JsonIgnore]
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
            HslColor = new HslColor(0, 0, 85); // default color
        }

        public Branch(List<CalcElement> elements) : this()
        {
            Elements = elements;
        }

        /// <summary>
        /// check if parameters have error for the dead end branch
        /// </summary>
        public void CheckParameterErrors()
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


        public void CreateBranches(List<string> branchConfig)
        {
            if (branchConfig.Count <= BranchLevel)
            {
                return;
            }

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
                    // set the parent tree and forest of the subbranch
                    ParentForest = this.ParentForest,
                    ParentTree = this.BranchLevel == 0 ? this as Tree : this.ParentTree,
                };
                SubBranches.Add(branch);
            }

            if (SubBranches.Count == 0)
            {
                return;
            }


            for (int index = 0; index < SubBranches.Count; index++)
            {
                var subBranch = SubBranches[index];
                subBranch.BranchLevel = BranchLevel + 1;
                subBranch.CreateBranches(branchConfig);
            }
        }

        /// <summary>
        /// set the assembly of the current branch. 
        /// Also set the assembly of all subbranches to the same value if they have no assembly assigned yet or the assembly is the same.
        /// calculate the branch with the new assembly if it is a dead end branch.
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

        public void ResetAssemblies()
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
        /// calculates the results for one branch (should be the end branch) with assembly assigned.
        /// </summary>
        public Branch Copy()
        {
            var branch = new Branch
            {
                Parameter = Parameter,
                Method = Method,
                Value = Value,
                BranchLevel = BranchLevel,
                Assemblies = Assemblies,
                HslColor = HslColor,
                Elements = Elements,
                ParentBranch = ParentBranch,
                ParentTree = ParentTree,
                ParentForest = ParentForest
            };
            if (SubBranches != null)
            {
                branch.SubBranches = SubBranches.Select(sb => sb.Copy()).ToList();
            }
            return branch;
        }

        /// <summary>
        /// add a new branch to the current branch using the parameter and value.
        /// returns the new branch.
        /// </summary>
        public Branch AddBranch(string parameter, string value, List<Assembly> assemblies)
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
                ParentTree = ParentTree,
                ParentForest = ParentForest
            };
            SubBranches.Add(newBranch);
            return newBranch;
        }

        /// <summary>
        /// This method returns a flat list of all subbranches. The current branch is included.
        /// Preferrably use this method on the root branch instance aka tree.Trunk.
        /// The output is useful when you want to iterate over all branches
        /// but don't care about the tree structure anymore.
        /// </summary>
        public List<Branch> Flatten()
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
