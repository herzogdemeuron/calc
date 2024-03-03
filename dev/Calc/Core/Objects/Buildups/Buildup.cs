using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private ObservableCollection<BuildupComponent> components = new ObservableCollection<BuildupComponent>();
        [JsonProperty("components")]
        public ObservableCollection<BuildupComponent> Components
        {
            get => components;
            set => SetProperty(ref components, value);
        }

        private ObservableCollection<BuildupComponent> missingComponents = new ObservableCollection<BuildupComponent>();
        [JsonProperty("missing_components")]
        public ObservableCollection<BuildupComponent> MissingComponents
        {
            get => missingComponents;
            set => SetProperty(ref missingComponents, value);
        }


        private Unit buildupUnit;
        [JsonProperty("unit")]
        public Unit BuildupUnit
        {
            get => buildupUnit;
            set => SetProperty(ref buildupUnit, value);
        }

        /// <summary>
        /// receive layerComponents from revit/rhino to the buildupcomponents,
        /// by checking if the layerComponent is targeted to the buildupcomponent.
        /// </summary>
        public void ReceiveLayerComponents(List<LayerComponent> layerComponents)
        {
            ResetComponentTargets();
            var unmatchedLayerComponents = MatchLayerComponents(layerComponents);
            foreach (var unmatched in unmatchedLayerComponents)
            {
                Components.Add(new BuildupComponent { LayerComponent = unmatched });
            }

        }

        /// <summary>
        /// match layerComponents to the buildupcomponents,
        /// if the buildupcomponent is not matched, add it to the missingComponents.
        /// return the leftover layerComponents.
        private List<LayerComponent> MatchLayerComponents(List<LayerComponent> layerComponents)
        {
            var result = new List<LayerComponent>(layerComponents);

            foreach (var component in components)
            {
                foreach (var layerComponent in layerComponents)
                {
                    if (component.CheckTarget(layerComponent))
                    {
                        component.SetTarget(layerComponent);
                        result.Remove(layerComponent);
                        break;
                    }
                }
                MissingComponents.Add(component);
            }
            return result;
        }

        private void ResetComponentTargets()
        {
            if (Components == null) return;
            foreach (var component in Components)
            {
              component.LayerComponent.ResetTarget();
            }
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
