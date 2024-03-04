using Calc.Core.Calculations;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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

        private string name;
        [JsonProperty("buildup_name")]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string source;
        [JsonProperty("source")]
        public string Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }

        private BuildupGroup group;
        [JsonProperty("group_id")]
        public BuildupGroup Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        private List<BuildupComponent> components = new List<BuildupComponent>();
        [JsonProperty("components")]
        public List<BuildupComponent> Components
        {
            get => components;
            set => SetProperty(ref components, value);
        }

        private List<BuildupComponent> missingComponents = new List<BuildupComponent>();
        [JsonProperty("missing_components")]
        public List<BuildupComponent> MissingComponents
        {
            get => missingComponents;
            set => SetProperty(ref missingComponents, value);
        }

        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => GetCalculationComponents();
        }


        private Unit buildupUnit;
        [JsonProperty("unit")]
        public Unit BuildupUnit
        {
            get => buildupUnit;
            set => SetProperty(ref buildupUnit, value);
        }


        /// <summary>
        /// receive a set of BuildupComponents from revit/rhino, modify the current buildup components
        /// current buildup component (source) --> new revit/rhino buildup component (target)
        /// if source is found, apply it to the target.
        /// add target to the new buildup components
        /// </summary>
        public void ReceiveBuildupComponents(List<BuildupComponent> newBuildupComponents)
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
        }

        /// <summary>
        /// get the total ratio of the whole buildup when generating calculation components
        /// is 1 divides the quantity of the normalizer of the buildup unit
        /// </summary>
        private double GetTotalRatio()
        {
            var normalizer = Components.Where(c => c.IsNormalizer).ToList();
            if (normalizer.Count != 1) return 0;
            var value = normalizer[0].TotalBasicParameterSet.GetQuantity(BuildupUnit).Value;
            if(value.HasValue)
            {
                return 1/value.Value;
            }
            return 0;
        }

        private List<CalculationComponent> GetCalculationComponents()
        {
            var calculationComponents = new List<CalculationComponent>();
            var totalRatio = GetTotalRatio();
            if (totalRatio != 0)
            {
                foreach(var component in Components)
                {
                    var calculationComponent = CalculationComponent.FromBuildupComponent(component, totalRatio);
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
            Components = other.Components;
            BuildupUnit = other.BuildupUnit;
        }

        public Buildup Copy()
        {
            return new Buildup()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                Components = Components,
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
