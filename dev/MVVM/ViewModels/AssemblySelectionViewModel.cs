using Calc.Core;
using Calc.Core.Objects.Assemblies;
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

    public class AssemblySelectionViewModel : INotifyPropertyChanged
    {

        private readonly CalcStore calcStore;
        public ICollectionView AllAssembliesView { get; }
        public List<StandardModel> AllStandards { get; }
        public List<AssemblyGroup> AllAssemblyGroups { get; }

        private string currentSearchText;
        private readonly AssemblyGroup defaultGroup = new AssemblyGroup() { Name = "All Groups", Id = 0 };

        private AssemblyGroup selectedAssemblyGroup;
        public AssemblyGroup SelectedAssemblyGroup
        {
            get => selectedAssemblyGroup;
            set
            {
                if (value == selectedAssemblyGroup) return;
                selectedAssemblyGroup = value;
                FilterAssembliesWithType();
                OnPropertyChanged(nameof(SelectedAssemblyGroup));
            }
        }

        private Assembly selectedAssembly;
        public Assembly SelectedAssembly
        {
            get => selectedAssembly;
            set
            {
                CanOk = value != null;
                if (value == selectedAssembly) return;
                selectedAssembly = value;
                OnPropertyChanged(nameof(SelectedAssembly));
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
        /// Only show verified assemblies
        /// </summary>
        public AssemblySelectionViewModel(CalcStore store)
        {
            calcStore = store;
            AllAssembliesView = CollectionViewSource.GetDefaultView(store.AssembliesAll.Where(b => b.Verified).ToList());
            AllStandards = new List<StandardModel>(store.StandardsAll.Select(s => new StandardModel(s)));
            AllAssemblyGroups = new List<AssemblyGroup>() { defaultGroup };
            AllAssemblyGroups.AddRange(store.AssemblyGroupsAll);
            SelectedAssemblyGroup = defaultGroup;            
        }

        public void PrepareAssemblySelection(Assembly assembly)
        {
            CurrentImage = null;
            ImageText = "Image Preview";

            currentSearchText = "";
            SelectedAssembly = assembly;
            SelectedAssemblyGroup = defaultGroup;
            FilterAssembliesWithType();
        }

        public void HandleSourceCheckChanged()
        {
            FilterAssembliesWithType();
        }

        private void FilterAssembliesWithType()
        {
            AllAssembliesView.Filter = (obj) =>
            {
                var assembly = obj as Assembly;
               
                // either name, material type or material type family contains the search text
                var name = assembly.Name?.ToLower() ?? "";
                var standards = assembly.StandardItems.Select(i => i.Standard.Name).ToList();
                var code = assembly.Code?.ToLower() ?? "";
                var description = assembly.Description?.ToLower() ?? "";
                var groupEqual = SelectedAssemblyGroup.Id == 0 || assembly.Group.Name == SelectedAssemblyGroup.Name;

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

        private async Task LoadAssemblyImageAsync(Assembly assembly)
        {
            if (assembly == null) return;

            // try to load image if should
            var imageItem = assembly.AssemblyImage;

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

            if (SelectedAssembly == null)
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


            var loadId = SelectedAssembly?.Id;
            await LoadAssemblyImageAsync(SelectedAssembly);

            // refresh the current image if the selected assembly has not changed
            if (SelectedAssembly?.Id != loadId) return;

            var imageData = SelectedAssembly.AssemblyImage?.ImageData;
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
            FilterAssembliesWithType();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
