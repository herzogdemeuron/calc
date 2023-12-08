using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Calc.Core.Color;
using System;
using Speckle.Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Calc.Core.Objects
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Unit
    {
        each, m, m2, m3
    }

    public  struct CalcElement
    {
        public string Id;
        public string TypeName;
        public Dictionary<string, object> Fields;
        private Dictionary<Unit, decimal?> _quantities;

        public CalcElement(string id,
            string type,
            Dictionary<string, object> fields,
            decimal? length,
            decimal? area,
            decimal? volume
            )
        {
            this.Id = id;
            this.TypeName = type;
            this.Fields = fields;
            this._quantities = new Dictionary<Unit, decimal?>
            {
                { Unit.each, 1 },
                { Unit.m, length },
                { Unit.m2, area },
                { Unit.m3, volume }
            };
        }

        public decimal? GetQuantityByUnit(Unit unit)
        {
            if (_quantities.TryGetValue(unit, out decimal? value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Unit not recognized: {unit}");
            }
        }
    }

    public class Project
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; } = -1;
        [JsonProperty(PropertyName = "project_number")]
        public string ProjectNumber { get; set; }
    }

    public class Result
    {
        // parent infos
        [JsonProperty(PropertyName = "forest")]
        public string Forest { get; set; }
        [JsonProperty(PropertyName = "tree")]
        public string Tree { get; set; }

        // element infos
        [JsonProperty(PropertyName = "element_id")]
        public string ElementId { get; set; }
        [JsonProperty(PropertyName = "element_type")]
        public string ElementType { get; set; }
        [JsonProperty(PropertyName = "element_unit")]
        public Unit ElementUnit { get; set; }
        [JsonProperty(PropertyName = "element_quantity")]
        public decimal ElementQuantity { get; set; }

        // building infos
        [JsonProperty(PropertyName = "buildup_name")]
        public string BuildupName { get; set; }
        [JsonProperty(PropertyName = "buildup_group")]
        public string GroupName { get; set; }
        [JsonProperty(PropertyName = "buildup_unit")]
        public Unit BuildupUnit { get; set; }

        // material infos
        [JsonProperty(PropertyName = "material_name")]
        public string MaterialName { get; set; }
        [JsonProperty(PropertyName = "material_category")]
        public string MaterialCategory { get; set; }
        [JsonProperty(PropertyName = "material_source")]
        public string MaterialSource { get; set; }
        [JsonProperty(PropertyName = "material_source_code")]
        public string MaterialSourceCode { get; set; }
        [JsonProperty(PropertyName = "material_gwp")]
        public decimal MaterialGwp { get; set; }
        [JsonProperty(PropertyName = "material_unit")]
        public Unit MaterialUnit { get; set; }
        [JsonProperty(PropertyName = "material_amount")]
        public decimal MaterialAmount { get; set; }

        // calculation results
        [JsonProperty(PropertyName = "calculated_gwp")]
        public decimal Gwp { get; set; }
        [JsonProperty(PropertyName = "calculated_cost")]
        public decimal Cost { get; set; }

        // others
        [JsonProperty(PropertyName = "color")]
        public HslColor Color { get; set; }
    }

    public class Snapshot : IHasProject
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; } = -1;
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "project_id", NullValueHandling = NullValueHandling.Ignore)]
        public Project Project { get; set; }
        [JsonProperty(PropertyName = "results")]
        public List<Result> Results { get; set; }
    }

    public class Material
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get;  set; } = -1;
        [JsonProperty("material_name")]
        public string Name { get;  set; }
        [JsonProperty("global_warming_potential_a1_a2_a3")]
        public decimal KgCO2eA123 { get;  set; }
        [JsonProperty("cost")]
        public decimal Cost { get; set; }
        [JsonProperty("unit")]
        public Unit Unit { get;  set; }
        [JsonProperty("material_category")]
        public string Category { get;  set; }
        [JsonProperty("source_db")]
        public string Source { get; set; }
        [JsonProperty("source_db_identifier")]
        public string SourceCode { get; set; }

        public override string ToString()
        {
            return $"Material Id: {Id}, Material Name: {Name}, KgCO2eA123: {KgCO2eA123}, Category: {Category}";
        }
    }

    public class BuildupComponent
    {
        [JsonProperty("calc_materials_id")]
        public Material Material { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonIgnore]
        public string FormattedAmount { get => Math.Round(Amount, 2).ToString() + " " + Material.Unit; }
        [JsonIgnore]
        public string FormattedKgCO2eA123 { get => Math.Round(Amount * Material.KgCO2eA123, 2).ToString() + " Kg"; }
        [JsonIgnore]
        public string FormattedCost { get => Math.Round(Amount * Material.Cost, 2).ToString(); }
    }

    public class Buildup : INotifyPropertyChanged
    {
        private int id = -1;
        private string name;
        private MaterialGroup group;
        private ObservableCollection<BuildupComponent> components;
        private Unit unit;

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

        [JsonProperty("components")]
        public ObservableCollection<BuildupComponent> Components
        {
            get => components;
            set => SetProperty(ref components, value);
        }

        [JsonProperty("unit")]
        public Unit Unit
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        public void Copy(Buildup other)
        {
            Id = other.Id;
            Name = other.Name;
            Group = other.Group;
            Components = other.Components;
            Unit = other.Unit;
        }

        public Buildup Copy()
        {
            return new Buildup()
            {
                Id = Id,
                Name = Name,
                Group = Group,
                Components = Components,
                Unit = Unit
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

    public class PathItem
    {
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (PathItem)obj;
            return Parameter == other.Parameter && Value == other.Value;
        }
        public override int GetHashCode()
        {
            return Parameter.GetHashCode() ^ Value.GetHashCode();
        }

    }

    public class MaterialGroup
    {
        [JsonProperty("group_name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Group Name: {Name}";
        }
    }
}
