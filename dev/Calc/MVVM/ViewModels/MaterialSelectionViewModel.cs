using Calc.Core;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Calc.MVVM.ViewModels
{

    public class MaterialSelectionViewModel : INotifyPropertyChanged
    {
        private readonly List<Material> allMaterials;
        public ICollectionView AllMaterialsView { get; set; }
        public List<StandardModel> AllStandards { get; set; }
        private List<FilterTagModel> materialTags;
        public ICollectionView MaterialTagsView
        {
            get
            {
                var view = CollectionViewSource.GetDefaultView(materialTags);
                if(view is ListCollectionView listCollectionView)
                {
                    listCollectionView.CustomSort = new FilterTagComparer();
                }
                return view;
            }
        }
        private List<FilterTagModel> productTags;
        public ICollectionView ProductTagsView
        {
            get
            {
                var view = CollectionViewSource.GetDefaultView(productTags);
                if (view is ListCollectionView listCollectionView)
                {
                    listCollectionView.CustomSort = new FilterTagComparer();
                }
                return view;
            }
        }
        private string currentSearchText;

        private Material selectedMaterial;
        public Material SelectedMaterial
        {
            get => selectedMaterial;
            set
            {
                if (value == selectedMaterial) return;
                selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        private FilterTagModel selectedMaterialTag;
        public FilterTagModel SelectedMaterialTag
        {
            get => selectedMaterialTag;
            set
            {
                selectedMaterialTag = value;
                FilterMaterialsWithType();
                UpdateDynamicCounts();
                OnPropertyChanged(nameof(SelectedMaterialTag));
            }
        }

        private FilterTagModel selectedProductTag;
        public FilterTagModel SelectedProductTag
        {
            get => selectedProductTag;
            set
            {
                selectedProductTag = value;
                FilterMaterialsWithType();
                UpdateDynamicCounts();
                OnPropertyChanged(nameof(SelectedProductTag));
            }
        }

        public MaterialSelectionViewModel(CalcStore store)
        {
            AllMaterialsView = CollectionViewSource.GetDefaultView(store.MaterialsAll);
            allMaterials = store.MaterialsAll;
            AllStandards = new List<StandardModel>(store.StandardsAll.Select(s => new StandardModel(s)));
            InitTypeTags();

        }

        public void Reset()
        {
            SelectedMaterialTag = null;
            SelectedProductTag = null;
            SelectedMaterial = null;
        }

        public void PrepareMaterialSelection(Material material)
        {
            currentSearchText = "";
            SelectedMaterialTag = null;
            SelectedProductTag = null;
            SelectedMaterial = material;
        }

        public void HandleSourceCheckChanged()
        {
            FilterMaterialsWithType();
        }

        private void InitTypeTags()
        {
            materialTags = new List<FilterTagModel>();
            productTags = new List<FilterTagModel>();
            foreach (var material in allMaterials)
            {
                var mType = material.MaterialTypeFamily ?? "?";
                var pType = material.ProductTypeFamily ?? "?";
                if (!materialTags.Exists(t => t.TagName == mType))
                {
                    materialTags.Add(new FilterTagModel(mType));
                }
                if (!productTags.Exists(t => t.TagName == pType))
                {
                    productTags.Add(new FilterTagModel(pType));
                }

                // add relation count to each other
                var mTag = materialTags.Find(t => t.TagName == mType);
                var pTag = productTags.Find(t => t.TagName == pType);
                mTag.AddRelationCount(pTag);
                pTag.AddRelationCount(mTag);

            }
        }

        private void FilterMaterialsWithType()
        {
            AllMaterialsView.Filter = (obj) =>
            {
                var material = obj as Material;
                // either name, material type or material type family contains the search text
                var name = material.Name?.ToLower() ?? "";
                var standard = material.Standard.Name ?? "";
                var mType = material.MaterialType?.ToLower() ?? "";
                var mFamily = material.MaterialTypeFamily?.ToLower() ?? "";
                var pType = material.ProductTypeFamily?.ToLower() ?? "";
                var pFamily = material.ProductTypeFamily?.ToLower() ?? "";

                if (!AllStandards.Where(s => s.IsSelected).Any(s => s.Name == standard))
                {
                    return false;
                }

                if (SelectedMaterialTag != null && (material.MaterialTypeFamily ??"?") != SelectedMaterialTag.TagName)
                {
                    return false;
                }
                if (SelectedProductTag != null && (material.ProductTypeFamily ?? "?") != SelectedProductTag.TagName)
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(currentSearchText) 
                    && !name.Contains(currentSearchText) 
                    && !mType.Contains(currentSearchText) 
                    && !mFamily.Contains(currentSearchText)
                    && !pType.Contains(currentSearchText)
                    && !pFamily.Contains(currentSearchText))
                {
                    return false;
                }
                return true;
            };
        }

        /// <summary>
        /// dynamic count is the count of each tag where the other type is selected
        /// if a tag of material type is selected, 
        /// the dynamic count of product type is the count of all materials with the selected material type.
        /// if a tag of the same type group is selected, all the other tags of the same type group will have null dynamic count
        /// the dynamic count is filtered with both selected tags
        /// </summary>
        private void UpdateDynamicCounts()
        {
            UpdateOtherGroupDynamicCounts(selectedProductTag, materialTags);
            UpdateOtherGroupDynamicCounts(selectedMaterialTag, productTags);
            UpdateSameGroupDynamicCounts(selectedMaterialTag, materialTags);
            UpdateSameGroupDynamicCounts(selectedProductTag, productTags);
        }

        private void UpdateOtherGroupDynamicCounts(FilterTagModel selectedTag, List<FilterTagModel> allTags)
        {
            foreach (var tag in allTags)
            {
                tag.UpdateDynamicCount(selectedTag);
            }
        }

        private void UpdateSameGroupDynamicCounts(FilterTagModel selectedTag, List<FilterTagModel> allTags)
        {
            foreach(var tag in allTags)
            {
                if (tag != selectedTag && selectedTag!=null)
                {
                    tag.CleanDynamicCount();
                }
            }
        }

        public void HandleSearchTextChanged(string currentText)
        {
            currentSearchText = currentText.ToLower();
            FilterMaterialsWithType();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
