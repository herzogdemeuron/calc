using Calc.Core.Color;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers;
using Calc.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Calc.MVVM.Models
{
    public class LayerComponentModel : INotifyPropertyChanged
    {
        private List<Material> materialsAll;
        private ICollectionView _allMaterialsView;
        public ICollectionView AllMaterialsView
        {
            get
            {
                if (materialsAll != null)
                {
                    _allMaterialsView = CollectionViewSource.GetDefaultView
                        (
                            new ObservableCollection<Material>(materialsAll)
                        );
                    if (_allMaterialsView.GroupDescriptions.Count == 0)
                    {
                        _allMaterialsView.GroupDescriptions.Add(new PropertyGroupDescription("Group.Name"));
                    }
                }
                return _allMaterialsView;
            }
        }

        public LayerComponentModel(List<Material> allMaterials)
        {
            materialsAll = allMaterials;
        }





        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
