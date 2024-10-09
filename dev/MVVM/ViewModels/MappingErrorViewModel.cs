using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.MVVM.ViewModels
{
    /// <summary>
    /// Takes care of broken mappings in calc project.
    /// </summary>
    public class MappingErrorViewModel : INotifyPropertyChanged
    {
        private readonly MappingViewModel mappingVM;
        public bool HasBrokenItems => BrokenNodeSource.Count > 0;
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

        /// <summary>
        /// Update the broken query set on the view with a new broken query set.
        /// </summary>
        internal void UpdateBrokenNodes(QueryTemplate brokenQuerySet)
        {
            if (brokenQuerySet == null) return;
            if(brokenQuerySet.Queries?.Count > 0)
            {
                BrokenQuerySet = brokenQuerySet;
                BrokenNodeSource.Clear();
                foreach (var query in brokenQuerySet.Queries)
                {
                    BrokenNodeSource.Add(new NodeModel(query));
                }
            }
            else
            {
                BrokenQuerySet = null;
                BrokenNodeSource.Clear();
            }
            mappingVM.BrokenQuerySet = BrokenQuerySet; // store to mappingVM to be able to upload if not ignored.
            BrokenSectionVisibility = (BrokenNodeSource.Count > 0)? Visibility.Visible: Visibility.Collapsed;
            OnPropertyChanged(nameof(BrokenSectionVisibility));
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        /// <summary>
        /// Adjusts the broken section visibility.
        /// </summary>
        internal void HandleMappingErrorClicked()
        {
            BrokenSectionVisibility = BrokenSectionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Displays the mapped assemblies for this node.
        /// </summary>
        internal void HandleBrokenNodeSelectionChanged(NodeModel nodeItem)
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

        /// <summary>
        /// Removes the selected node.
        /// Hides the broken section if no broken items are present.
        /// </summary>
        internal void RemoveBrokenNode(NodeModel nodeItem)
        {
            if(nodeItem == null) return;
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

        internal void RemoveAllBrokenNodes()
        {
            BrokenNodeSource.Clear();
            Assembly1 = null;
            Assembly2 = null;
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
