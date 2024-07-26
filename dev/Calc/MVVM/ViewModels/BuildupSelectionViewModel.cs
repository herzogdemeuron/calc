using Calc.Core;
using Calc.Core.Objects.Buildups;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Calc.MVVM.ViewModels
{

    public class BuildupSelectionViewModel : INotifyPropertyChanged
    {

        private readonly CalcStore calcStore;
        public ICollectionView AllBuildupsView { get; }
        public List<StandardModel> AllStandards { get; }
        public List<BuildupGroup> AllBuildupGroups { get; }

        private string currentSearchText;
        private readonly BuildupGroup defaultGroup = new BuildupGroup() { Name = "All Groups", Id = 0 };

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

        private BitmapImage currentImage;
        public BitmapImage CurrentImage
        {
            get => currentImage;
            set
            {
                if (value == currentImage) return;
                currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get => loadingVisibility;
            set
            {
                if (value == loadingVisibility) return;
                loadingVisibility = value;
                OnPropertyChanged(nameof(LoadingVisibility));
            }
        }

        private string imageText;
        public string ImageText
        {
            get => imageText;
            set
            {
                if (value == imageText) return;
                imageText = value;
                OnPropertyChanged(nameof(ImageText));
            }
        }

        /// <summary>
        /// Only show verified buildups
        /// </summary>
        public BuildupSelectionViewModel(CalcStore store)
        {
            calcStore = store;
            AllBuildupsView = CollectionViewSource.GetDefaultView(store.BuildupsAll.Where(b => b.Verified).ToList());
            AllStandards = new List<StandardModel>(store.StandardsAll.Select(s => new StandardModel(s)));
            AllBuildupGroups = new List<BuildupGroup>() { defaultGroup };
            AllBuildupGroups.AddRange(store.BuildupGroupsAll);
            SelectedBuildupGroup = defaultGroup;            
        }

        public void PrepareBuildupSelection(Buildup buildup)
        {
            CurrentImage = null;
            ImageText = "Image Preview";

            currentSearchText = "";
            SelectedBuildup = buildup;
            SelectedBuildupGroup = defaultGroup;
            FilterBuildupsWithType();
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

        private async Task LoadBuildupImageAsync(Buildup buildup)
        {
            if (buildup == null) return;

            // try to load image if should
            var imageItem = buildup.BuildupImage;

            if (imageItem != null && imageItem.Id != null && !imageItem.ImageLoaded)
            {
                var data = await calcStore.LoadImageAsync(imageItem.Id);
                imageItem.ImageData = data;
                imageItem.ImageLoaded = true;
            }
        }

        public async Task HandleBuilupSelectionChangedAsync()
        {

            CurrentImage = null;
            ImageText = "";

            if (SelectedBuildup == null)
            {
                LoadingVisibility = Visibility.Collapsed;
                ImageText = "Image Preview";
                return;
            }
            else
            {
                LoadingVisibility = Visibility.Visible;
                ImageText = "";
            }


            var loadId = SelectedBuildup?.Id;
            await LoadBuildupImageAsync(SelectedBuildup);

            // refresh the current image if the selected buildup has not changed
            if (SelectedBuildup?.Id != loadId) return;

            var imageData = SelectedBuildup.BuildupImage?.ImageData;
            if (imageData != null)
            {
                CurrentImage = ImageHelper.ByteArrayToBitmap(imageData);
            }
            else
            {
                LoadingVisibility = Visibility.Collapsed;
                ImageText = "No Image Available";
            }
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
