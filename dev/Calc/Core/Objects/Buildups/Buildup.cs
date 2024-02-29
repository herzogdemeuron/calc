using Calc.Core.Objects.Materials;
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
        private string name;
        private MaterialGroup group;
        private ObservableCollection<BuildupComponent> components;
        private Unit buildupUnit;

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [JsonProperty("buildup_name")]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        [JsonProperty("group_id")]
        public MaterialGroup Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("components")]
        public ObservableCollection<BuildupComponent> Components
        {
            get => components;
            set => SetProperty(ref components, value);
        }

        [JsonProperty("unit")]
        public Unit BuildupUnit
        {
            get => buildupUnit;
            set => SetProperty(ref buildupUnit, value);
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    }
}
