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
        public ICollectionView AllBuildupsView { get; }
        public List<StandardModel> AllStandards { get; }
        public List<BuildupGroup> AllBuildupGroups { get; }

        private string currentSearchText;
        private BuildupGroup defaultGroup = new BuildupGroup() { Name = "All Groups", Id = 0 };

        private BuildupGroup selectedBuildupGroup;
        public BuildupGroup SelectedBuildupGroup
        {
            get => selectedBuildupGroup;
            set
            {
                if (value == selectedBuildupGroup) return;
                selectedBuildupGroup = value;
                FilterBuildupsWithType();
                OnPropertyChanged(nameof(SelectedBuildupGroup));
            }
        }

        private Buildup selectedBuildup;
        public Buildup SelectedBuildup
        {
            get => selectedBuildup;
            set
            {
                CanOk = value != null;
                if (value == selectedBuildup) return;
                selectedBuildup = value;
                OnPropertyChanged(nameof(SelectedBuildup));
            }
        }

        private bool canOk;
        public bool CanOk
        {
            get => canOk;
            set
            {
                if (value == canOk) return;
                canOk = value;
                OnPropertyChanged(nameof(CanOk));
            }
        }

        public BuildupSelectionViewModel(DirectusStore store)
        {
            AllBuildupsView = CollectionViewSource.GetDefaultView(store.BuildupsAll);
            AllStandards = new List<StandardModel>(store.StandardsAll.Select(s => new StandardModel(s)));

            AllBuildupGroups = new List<BuildupGroup>() { defaultGroup };
            AllBuildupGroups.AddRange(store.BuildupGroupsAll);
            SelectedBuildupGroup = defaultGroup;
        }

        public void Reset()
        {
            SelectedBuildup = null;
            SelectedBuildupGroup = defaultGroup;
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
                var description = buildup.Description?.ToLower() ?? "";
                var groupEqual = SelectedBuildupGroup.Id == 0 || buildup.Group.Name == SelectedBuildupGroup.Name;

                if (!groupEqual) return false;

                if (!AllStandards.Where(s => !s.IsSelected).All(s => !standards.Contains(s.Name)))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(currentSearchText)
                    && !name.Contains(currentSearchText)
                    && !code.Contains(currentSearchText))

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
