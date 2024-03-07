using Calc.Core.Calculations;
using Calc.Core.Color;
using Calc.Core.Helpers;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Mappings;
using Calc.Core.Objects.Results;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calc.Core.Objects.GraphNodes
{
    public class Branch : IGraphNode, INotifyPropertyChanged
    {
        [JsonIgnore]
        public List<CalcElement> Elements { get; set; } = new();
        [JsonIgnore]
        public double TotalArea 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m2).Amount ?? 0
                    ),
                0); 
        }
        [JsonIgnore]
        public double TotalVolume 
        { 
            get => Math.Round(
                Elements.Sum(
                    e => e.GetBasicUnitParameter(Unit.m3).Amount ?? 0
                    ),
                0); 
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
        private ObservableCollection<Buildup> _buildups = new();
        [JsonIgnore]
        public ObservableCollection<Buildup> Buildups
        {
            // set _buildups to empty list if null to avoid null reference exceptions
            get => _buildups;
            private set
            {
                _buildups = value;
                if (SubBranches.Count == 0)
                {
                    Calculator.Calculate(this);
                }
                OnPropertyChanged(nameof(Buildups));
            }
        }
        [JsonIgnore]
        public string BuildupsIdentifier
        {
            get
            {
                return GetBuildupsIdentifier(Buildups.ToList());
            }
        }
        private HslColor _hslColor;
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
        public bool IsFullyCalculated
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
        public bool HasCalculationResults => (CalculationResults != null && CalculationResults.Count > 0);

        private List<Result> _calculationResults = new();
        [JsonIgnore]
        public List<Result> CalculationResults
        {
            get
            {
                if (SubBranches.Count > 0)
                {
                    return SubBranches.SelectMany(sb => sb.CalculationResults ?? new List<Result>()).ToList();
                }
                return _calculationResults;
            }
            set
            {
                _calculationResults = value;
                OnPropertyChanged(nameof(CalculationResults));
            }
        }


        public Branch()
        {
            Parameter = "No Parameter";
            Method = "No Method";
            Value = "No Value";
            HslColor = new HslColor(0, 0, 85);
        }

        public Branch(List<CalcElement> elements) : this()
        {
            Elements = elements;
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
                    ParentForest = ParentForest,
                    ParentTree = BranchLevel == 0 ? this as Tree : ParentTree,
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
        /// set the buildup of the current branch. 
        /// Also set the buildup of all subbranches to the same value if they have no buildup assigned yet or the buildup is the same.
        /// calculate the branch with the new buildup if it is a dead end branch.
        /// </summary>
        public void SetBuildups(List<Buildup> buildups)
        {
            if (buildups == null) return;
            var newIdentifier = GetBuildupsIdentifier(buildups);
            var currentIdentifier = BuildupsIdentifier;
            if (currentIdentifier == newIdentifier) return;

            Buildups = new(buildups);

            OnPropertyChanged(nameof(Buildups));
            foreach (var subBranch in SubBranches)
            {
                if (subBranch.Buildups == null || subBranch.BuildupsIdentifier == currentIdentifier)
                {
                    subBranch.SetBuildups(buildups);
                }
            }
        }

        public void ResetBuildups()
        {
            Buildups = new();
            if (SubBranches.Count == 0)
            {
                return;
            }
            foreach (var subBranch in SubBranches)
            {
                subBranch.ResetBuildups();
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
            if (ParentBranch.Buildups != null)
            {
                SetBuildups(ParentBranch.Buildups.ToList());
            }
        }

        /// <summary>
        /// calculates the results for one branch (should be the end branch) with buildup assigned.
        /// </summary>
        public Branch Copy()
        {
            var branch = new Branch
            {
                Parameter = Parameter,
                Method = Method,
                Value = Value,
                BranchLevel = BranchLevel,
                Buildups = Buildups,
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
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Branch AddBranch(string parameter, string value, List<Buildup> buildups)
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
                Buildups = new ObservableCollection<Buildup>(buildups),
                ParentBranch = this,
                BranchLevel = this.BranchLevel + 1,
                ParentTree = ParentTree,
                ParentForest = ParentForest
            };
            SubBranches.Add(newBranch);
            return newBranch;
        }

        /// <summary>
        /// WARNING: DESTRUCTIVE METHOD - USE ONLY ON A COPY OF THE TREE
        /// The Intended use is right befor calculation.
        /// 
        /// This method removes elements from the current branch that
        /// are also present in subbranches that have a buildup assigned.
        /// In the bigger picture, this allows to override buildups further down the tree.
        /// </summary>
        public void RemoveElementsByBuildupOverrides()
        {
            if (Buildups != null && SubBranches.Count > 0)
            {
                // get all elements with buildup from subbranches
                var subElementsWithBuildup = SubBranches
                    .Where(sb => sb.Buildups != null)
                    .Where(sb => sb.Buildups.Count > 0)
                    .SelectMany(sb => sb.Elements)
                    .ToList();
                // remove subelements with buildup from elements of current branch
                Elements = Elements
                    .Where(e => !subElementsWithBuildup.Contains(e))
                    .ToList();
            }
            if (SubBranches.Count > 0)
            {
                foreach (var subBranch in SubBranches)
                {
                    subBranch.RemoveElementsByBuildupOverrides();
                }
            }
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

        public void PrintTree(int indentLevel = 0)
        {
            string indentation = new(' ', indentLevel * 4);
            Console.WriteLine($"{indentation}∟: Elements: {Elements.Count}, Param: {Parameter}, Value: {Value}, Method: {Method}, Color: {HslColor.H}, Buildup: {Buildups}");

            foreach (var subBranch in SubBranches)
            {
                subBranch.PrintTree(indentLevel + 1);
            }
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

        private string GetBuildupsIdentifier(List<Buildup> buildups)
        {
            if (buildups == null)
                return null;
            return string.Join(",", buildups.Select(b => b?.Id??'-').OrderBy(i => i));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
