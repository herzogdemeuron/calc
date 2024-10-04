using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using Calc.MVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.MVVM.ViewModels
{

    public class MappingErrorViewModel : INotifyPropertyChanged
    {
        public bool HasBrokenItems => BrokenNodeSource.Count > 0;
        private MappingViewModel mappingVM;
        public ObservableCollection<NodeModel> BrokenNodeSource {  get; private set; }

        private string _assembly1;
        public string Assembly1
        {
            get => _assembly1;
            set
            {
                _assembly1 = value;
                OnPropertyChanged(nameof(Assembly1));
            }
        }

        private string _assembly2;
        public string Assembly2
        {
            get => _assembly2;
            set
            {
                _assembly2 = value;
                OnPropertyChanged(nameof(Assembly2));
            }
        }

        private QueryTemplate _brokenQuerySet;
        public QueryTemplate BrokenQuerySet
        {
            get => _brokenQuerySet;
            set
            {
                _brokenQuerySet = value;
                OnPropertyChanged(nameof(BrokenQuerySet));
            }
        }

        private Visibility _brokenSectionVisibility = Visibility.Collapsed;
        public Visibility BrokenSectionVisibility
        {
            get => _brokenSectionVisibility;
            set
            {
                _brokenSectionVisibility = value;
                OnPropertyChanged(nameof(BrokenSectionVisibility));
            }
        }

        public MappingErrorViewModel(MappingViewModel mappingViewModel)
        {
            mappingVM = mappingViewModel;
            BrokenNodeSource = new ObservableCollection<NodeModel>();
        }

        internal void UpdateBrokenNodes(QueryTemplate qryTemplate)
        {
            if (qryTemplate == null) return;

            if(qryTemplate.Queries?.Count > 0)
            {
                BrokenQuerySet = qryTemplate;
                BrokenNodeSource.Clear();
                foreach (var query in qryTemplate.Queries)
                {
                    BrokenNodeSource.Add(new NodeModel(query));
                }
            }
            else
            {
                BrokenQuerySet = null;
                BrokenNodeSource.Clear();
            }

            mappingVM.BrokenQuerySet = BrokenQuerySet;
            BrokenSectionVisibility = (BrokenNodeSource.Count > 0)? Visibility.Visible: Visibility.Collapsed;
            OnPropertyChanged(nameof(BrokenSectionVisibility));
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public void HandleMappingErrorClicked()
        {
            BrokenSectionVisibility = BrokenSectionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public void HandleBrokenNodeSelectionChanged(NodeModel nodeItem)
        {
            if (nodeItem != null && nodeItem.Host!= null)
            {
                if(nodeItem.Host is Query)
                {
                    Assembly1 = "-";
                    Assembly2 = null;
                }
                else
                {
                    var branch = nodeItem.Host as Branch;
                    Assembly1 = branch.Assemblies?.Count > 0 ? branch.Assemblies[0].ToString() : "-";
                    Assembly2 = branch.Assemblies?.Count > 1 ? branch.Assemblies[1].ToString() : null;
                }
            }
            else
            {
                Assembly1 = null;
                Assembly2 = null;
            }
        }

        public void RemoveBrokenNode(NodeModel nodeItem)
        {
            if(nodeItem == null)
                return;


            if(nodeItem.Host is Query)
            {
                BrokenNodeSource.Remove(nodeItem);
                BrokenQuerySet.Queries.Remove(nodeItem.Host as Query);
            }
            else
            {
                nodeItem.RemoveFromParent();
            }

            Assembly1 = null;
            Assembly2 = null;

            // remove empty queries
            foreach (var query in BrokenNodeSource)
            {
                if (query.SubNodeItems?.Count == 0)
                {
                    BrokenNodeSource.Remove(query);
                    BrokenQuerySet.Queries.Remove(query.Host as Query);
                    break;
                }
            }

            if (BrokenNodeSource.Count == 0)
            {
                BrokenSectionVisibility = Visibility.Collapsed;
            }

            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
            OnPropertyChanged(nameof(BrokenSectionVisibility));
        }

        public void ResetAssemblies()
        {
            Assembly1 = null;
            Assembly2 = null;
        }

            public void RemoveAllBrokenNodes()
        {
            BrokenNodeSource.Clear();
            ResetAssemblies();
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
