using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Windows.Data;

namespace Calc.MVVM.Models
{
    public class LayerModel : INotifyPropertyChanged
    {
        private LayerComponent layer;
        private readonly ObservableCollection<Material> materialsFromStandard;
        public List<MaterialFunction> MaterialFunctionsAll { get; }
        public Material CurrentMaterial { get; set; }
        public string MaterialMatchInfo { get => GetMaterialMatchText(); }
        public List<Material> CurrentMaterials
        {
            get
            {
                var materials = new List<Material>();
                if (layer.HasMainMaterial) materials.Add(MainMaterial);
                if (layer.HasSubMaterial) materials.Add(SubMaterial);
                return materials;
            }
        }

        private MaterialFunction? selectedFunction;
        public MaterialFunction? SelectedFunction
        {
            get => selectedFunction;
            set
            {
                selectedFunction = value;
                OnPropertyChanged(nameof(SelectedFunction));
            }
        }

        public Material MainMaterial
        {
            get => layer.MainMaterial;
            set
            {
                layer.SetMainMaterial(value);
                OnPropertyChanged(nameof(MainMaterial));
                NotifyPropertiesChange();
            }
        }

        public Material SubMaterial
        {
            get => layer.SubMaterial;
            set
            {
                layer.SetSubMaterial(value);
                OnPropertyChanged(nameof(SubMaterial));
                NotifyPropertiesChange();
            }
        }

        public double SubMaterialRatio
        {
            get => layer.SubMaterialRatio;
            set
            {
                layer.SetSubMaterialRatio(value);
                OnPropertyChanged(nameof(SubMaterialRatio));
            }
        }

        private ICollectionView _allMaterialsView1;
        public ICollectionView AllMaterialsView1
        {
            get
            {
                if (materialsFromStandard != null)
                {
                    _allMaterialsView1 = CollectionViewSource.GetDefaultView(materialsFromStandard);
                    var groupDescrip = _allMaterialsView1.GroupDescriptions;
                    if (groupDescrip.Count == 0) groupDescrip.Add(new PropertyGroupDescription("GroupName"));
                }
                return _allMaterialsView1;
            }
        }

        private ICollectionView _allMaterialsView2;
        public ICollectionView AllMaterialsView2
        {
            get
            {
                if (materialsFromStandard != null)
                {
                    _allMaterialsView2 = CollectionViewSource.GetDefaultView(materialsFromStandard);
                    var groupDescrip = _allMaterialsView2.GroupDescriptions;
                    if (groupDescrip.Count == 0) groupDescrip.Add(new PropertyGroupDescription("GroupName"));
                }
                return _allMaterialsView2;
            }
        }

        public bool CanAddSecondMaterial
        {
            get
            {
                if (layer.HasMainMaterial)
                {
                    return true;
                }
                return false;
            }
        }

        public LayerModel(LayerComponent layercompo, List<Material> allMaterials, List<MaterialFunction> materialFunctionsAll)
        {
            layer = layercompo;
            materialsFromStandard = new ObservableCollection<Material>(allMaterials);
            MaterialFunctionsAll = materialFunctionsAll;
        }

        public void NotifyPropertiesChange()
        {
            OnPropertyChanged(nameof(CanAddSecondMaterial));
            OnPropertyChanged(nameof(CurrentMaterials));
            OnPropertyChanged(nameof(MaterialMatchInfo));
        }

        public void RemoveSubMaterial()
        {
            SubMaterial = null;
            SubMaterialRatio = 0;
        }

        private string GetMaterialMatchText()
        {
            if (layer.HasMainMaterial && layer.HasSubMaterial && MainMaterial.MaterialUnit != SubMaterial.MaterialUnit)
            {
                return $"Sub Material Unit ({SubMaterial.MaterialUnit}) does not match {MainMaterial.MaterialUnit}";
            }
            return null;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
