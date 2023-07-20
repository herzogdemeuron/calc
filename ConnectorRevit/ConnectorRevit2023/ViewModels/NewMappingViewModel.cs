using Calc.Core;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.Generic;

namespace Calc.ConnectorRevit.ViewModels
{
    public class NewMappingViewModel : INotifyPropertyChanged
    {
        private DirectusStore store;

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
            List<Mapping> allMappings = directusStore.MappingsAll;
            MappingsView = CollectionViewSource.GetDefaultView(allMappings);
            MappingsView.GroupDescriptions?.Add(new PropertyGroupDescription("Project.ProjectNumber"));
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
