using Calc.Core;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Materials;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Calc.MVVM.ViewModels
{

    public class BuildupSelectionViewModel : INotifyPropertyChanged
    {
        public ICollectionView AllBuildupsView { get; set; }
        public List<StandardModel> AllStandards { get; set; }
        
        private string currentSearchText;

        private Buildup selectedBuildup;
        public Buildup SelectedBuildup
        {
            get => selectedBuildup;
            set
            {
                if (value == selectedBuildup) return;
                selectedBuildup = value;
                OnPropertyChanged(nameof(SelectedBuildup));
            }
        }

        public BuildupSelectionViewModel(DirectusStore store)
        {
            AllBuildupsView = CollectionViewSource.GetDefaultView(store.BuildupsAll);
            AllStandards = new List<StandardModel>(store.StandardsAll.Select(s => new StandardModel(s)));

        }

        public void Reset()
        {
            SelectedBuildup = null;
        }

        public void PrepareBuildupSelection(Buildup buildup)
        {
            currentSearchText = "";
            SelectedBuildup = buildup;
        }

        public void HandleSourceCheckChanged()
        {
            FilterBuildupsWithType();
        }


        private void FilterBuildupsWithType()
        {
            AllBuildupsView.Filter = (obj) =>
            {
                var buildup = obj as Buildup;
               
                // either name, material type or material type family contains the search text
                var name = buildup.Name?.ToLower() ?? "";
                var standards = buildup.StandardItems.Select(i => i.Standard.Name).ToList();
                var code = buildup.Code?.ToLower() ?? "";
                var group = buildup.Group.Name ?? "";


                /*if (!AllStandards.Where(s => s.IsSelected).Any(s => s.Name == standard))
                {
                    return false;
                }*/

                if (!string.IsNullOrEmpty(currentSearchText)
                    && !name.Contains(currentSearchText)
                    && !code.Contains(currentSearchText)
                    && !group.Contains(currentSearchText))

                {
                    return false;
                }
                return true;
            };
        }


        public void HandleSearchTextChanged(string currentText)
        {
            currentSearchText = currentText.ToLower();
            FilterBuildupsWithType();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
