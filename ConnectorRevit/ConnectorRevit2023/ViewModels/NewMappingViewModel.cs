using Calc.Core;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NewMappingViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;
        private ObservableCollection<Mapping> allMappings;
        public ObservableCollection<Mapping> AllMappings
        {
            get { return allMappings; }
            set
            {
                allMappings = value;
                OnPropertyChanged(nameof(AllMappings));
            }
        }

        private string newName;
        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        private ICollectionView _mappingsView;
        public ICollectionView MappingsView
        {
            get { return _mappingsView; }
            set
            {
                _mappingsView = value;
                OnPropertyChanged(nameof(MappingsView));
            }
        }

        public NewMappingViewModel(DirectusStore directusStore)
        {
            store = directusStore;
            AllMappings = new ObservableCollection<Mapping>(directusStore.MappingsAll);
            MappingsView = CollectionViewSource.GetDefaultView(AllMappings);
            MappingsView.GroupDescriptions?.Add(new PropertyGroupDescription("Project.ProjectNumber"));
            //Debug.WriteLine(AllMappings.First().Project.ProjectNumber);
        }

        public async Task HandelNewMappingCreate(Mapping selectedMapping, string newName)
        {
            Mapping newMapping = selectedMapping.Copy(newName);
            store.MappingSelected = newMapping;
            await store.SaveSelectedMapping();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
