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
        private readonly List<Material> materialsAll;
        public List<MaterialFunction> MaterialFunctionsAll { get; }
        public Material CurrentMaterial { get; set; }
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

        private MaterialFunction materialFunction;
        public MaterialFunction MaterialFunction
        {
            get => materialFunction;
            set
            {
                materialFunction = value;
                OnPropertyChanged(nameof(MaterialFunction));
            }
        }

        public Material MainMaterial
        {
            get => layer.MainMaterial;
            set
            {
                layer.SetMainMaterial(value);
                OnPropertyChanged(nameof(MainMaterial));
                OnPropertyChanged(nameof(CanAddSecondMaterial));
                OnPropertyChanged(nameof(CurrentMaterials));
            }
        }

        public Material SubMaterial
        {
            get => layer.SubMaterial;
            set
            {
                layer.SetSubMaterial(value);
                OnPropertyChanged(nameof(SubMaterial));
                OnPropertyChanged(nameof(CanAddSecondMaterial));
                OnPropertyChanged(nameof(CurrentMaterials));
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
                if (materialsAll != null)
                {
                    _allMaterialsView1 = CollectionViewSource.GetDefaultView
                        (
                            new ObservableCollection<Material>(materialsAll)
                        );
                }
                return _allMaterialsView1;
            }
        }

        private ICollectionView _allMaterialsView2;
        public ICollectionView AllMaterialsView2
        {
            get
            {
                if (materialsAll != null)
                {
                    _allMaterialsView2 = CollectionViewSource.GetDefaultView
                        (
                            new ObservableCollection<Material>(materialsAll)
                        );
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
            materialsAll = allMaterials;
            MaterialFunctionsAll = materialFunctionsAll;
        }

        public void RemoveSubMaterial()
        {
            SubMaterial = null;
            SubMaterialRatio = 0;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
