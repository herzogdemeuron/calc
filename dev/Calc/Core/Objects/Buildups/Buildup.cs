using Calc.Core.Calculations;
using Calc.Core.Objects.Results;
using Calc.Core.Objects.Standards;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Calc.Core.Objects.Buildups
{
    public class Buildup : INotifyPropertyChanged, ISearchable  // check: notify property needed?
    {

        private int id = -1;
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [JsonProperty("standards")]
        public List<StandardItem> StandardItems { get; set; } // this is only for deserialization

        //[JsonIgnore]
        //public List<LcaStandard> Standards { get => StandardItems.Select(x => x.Standard).ToList(); }

        private string code;
        [JsonProperty("code")]
        public string Code
        {
            get => code;
            set => SetProperty(ref code, value);
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

        public string GroupName => Group?.Name;

        [JsonProperty("carbon_a1a3")]
        public double? BuildupGwp { get; set; }

        [JsonProperty("grey_energy_fabrication_total")]
        public double? BuildupGe { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string ImageUuid { get; set; }

        [JsonIgnore]
        public List<LayerResult> LayerSnapshot { get; set; }


        /*
        public void LinkGroup(List<BuildupGroup> buildupGroups) // deprecated
        {
            if (Group != null)
            {
                Group = buildupGroups.Find(g => g.Id == Group.Id);
            }
        }

        public void LinkStandard(List<LcaStandard> standards) // deprecated
        {
            if (Standard != null)
            {
                Standard = standards.Find(s => s.Id == Standard.Id);
            }
        }*/

        private List<CalculationComponent> calculationComponents = new List<CalculationComponent>();
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => calculationComponents;
            set => SetProperty(ref calculationComponents, value);
        }

       /* public void Copy(Buildup other) // deprecated
        {
            Id = other.Id;
            Name = other.Name;
            Group = other.Group;
            CalculationComponents = other.CalculationComponents;
            BuildupUnit = other.BuildupUnit;
        }

        public Buildup Copy() // deprecated
        {
            return new Buildup()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                CalculationComponents = CalculationComponents,
                BuildupUnit = BuildupUnit
            };
        }*/
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) // is this needed?
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
