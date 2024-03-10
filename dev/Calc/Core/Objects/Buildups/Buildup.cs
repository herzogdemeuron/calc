using Calc.Core.Calculations;
using Calc.Core.Objects.Materials;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Calc.Core.Objects.Buildups
{
    public class Buildup : INotifyPropertyChanged
    {

        private int id = -1;
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        private Standard standard;
        [JsonProperty("standard")]
        public Standard Standard
        {
            get => standard;
            set => SetProperty(ref standard, value);
        }

        private string name;
        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private Unit buildupUnit;
        [JsonProperty("buildup_unit")]
        public Unit BuildupUnit
        {
            get => buildupUnit;
            set => SetProperty(ref buildupUnit, value);
        }

        private BuildupGroup group;
        [JsonProperty("group")]
        public BuildupGroup Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }


        [JsonProperty("description")]
        public string Description { get; set; }

        private List<BuildupComponent> buildupComponents = new List<BuildupComponent>();
        public List<BuildupComponent> BuildupComponents
        {
            get => buildupComponents;
            set => SetProperty(ref buildupComponents, value);
        }

        public void LinkGroup(List<BuildupGroup> buildupGroups)
        {
            if (Group != null)
            {
                Group = buildupGroups.Find(g => g.Id == Group.Id);
            }
        }

        public void LinkStandard(List<Standard> standards)
        {
            if (Standard != null)
            {
                Standard = standards.Find(s => s.Id == Standard.Id);
            }
        }

        private List<CalculationComponent> calculationComponents = new List<CalculationComponent>();
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => calculationComponents;
            set => SetProperty(ref calculationComponents, value);
        }



        /// <summary>
        /// receive a set of BuildupComponents from revit/rhino, create the current buildup components
        /// </summary>
       /* public void CreateBuildupComponents(List<BuildupComponent> newBuildupComponents)
        {
            var currentComponents = new List<BuildupComponent>(Components);
            Components = newBuildupComponents;

            foreach (var newComponent in Components)
            {
                var source = currentComponents.FirstOrDefault(c => c.CheckSource(newComponent));
                if (source != null)
                {
                    var missingComponent = newComponent.ApplySource(source);
                    if (missingComponent.HasLayers)
                    {
                        MissingComponents.Add(missingComponent);
                    }

                    currentComponents.Remove(source);
                }
            }
            MissingComponents.AddRange(currentComponents);
        }*/

        /// <summary>
        /// get the total ratio of the whole buildup when generating calculation components
        /// is 1 divides the quantity of the normalizer of the buildup unit
        /// </summary>
        private double GetQuantityRatio()
        {
            var normalizer = BuildupComponents.Where(c => c.IsNormalizer).ToList();
            if (normalizer.Count != 1) return 0;
            var value = normalizer[0].BasicParameterSet.GetAmountParam(BuildupUnit).Amount;
            if(value.HasValue)
            {
                return 1/value.Value;
            }
            return 0;
        }

        public List<CalculationComponent> UpdateCalculationComponents()
        {
            var calculationComponents = new List<CalculationComponent>();
            var quantityRatio = GetQuantityRatio();
            if (quantityRatio != 0)
            {
                foreach(var component in BuildupComponents)
                {
                    var calculationComponent = CalculationComponent.FromBuildupComponent(component, quantityRatio);
                    calculationComponents.AddRange(calculationComponent);
                }
            }
            return calculationComponents;
        }

        public void Copy(Buildup other)
        {
            Id = other.Id;
            Name = other.Name;
            Group = other.Group;
            BuildupComponents = other.BuildupComponents;
            BuildupUnit = other.BuildupUnit;
        }

        public Buildup Copy()
        {
            return new Buildup()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                BuildupComponents = BuildupComponents,
                BuildupUnit = BuildupUnit
            };
        }
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Buildup other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
