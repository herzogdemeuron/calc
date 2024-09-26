using Calc.Core.Calculation;
using Calc.Core.Objects.Standards;
using Calc.Core.Snapshots;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Calc.Core.Objects.Assemblies
{
    public class Assembly : INotifyPropertyChanged, ISearchable  // check: notify property needed?
    {

        private int id;
        [JsonProperty("id")]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [JsonProperty("standards")]
        public List<StandardItem> StandardItems { get; set; } // this is only for deserialization

        [JsonIgnore]
        public string StandardsString { get => StandardItems.Select(x => x.Standard.Name).Aggregate((x, y) => x + ", " + y); }

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

        private Unit assemblyUnit;
        [JsonProperty("assembly_unit")]
        public Unit AssemblyUnit
        {
            get => assemblyUnit;
            set => SetProperty(ref assemblyUnit, value);
        }

        private AssemblyGroup group;
        [JsonProperty("group")]
        public AssemblyGroup Group
        {
            get => group;
            set => SetProperty(ref group, value);
        }

        public string GroupName => Group?.Name;

        [JsonProperty("carbon_a1a3")]
        public double? AssemblyGwp { get; set; }

        [JsonProperty("grey_energy_fabrication_total")]
        public double? AssemblyGe { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("image")]
        public AssemblyImage AssemblyImage { get; set; }
        [JsonProperty("speckle_project_id")]
        public string SpeckleProjectId { get; set; }
        [JsonProperty("speckle_model_id")]
        public string SpeckleModelId { get; set; }

        [JsonIgnore]
        public List<AssemblySnapshot> AssemblySnapshot { get; set; }


        private List<CalculationComponent> calculationComponents = new List<CalculationComponent>();
        [JsonProperty("calculation_components")]
        public List<CalculationComponent> CalculationComponents
        {
            get => calculationComponents;
            set => SetProperty(ref calculationComponents, value);
        }

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
            if (obj is Assembly other)
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
