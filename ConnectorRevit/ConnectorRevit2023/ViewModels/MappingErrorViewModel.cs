using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.Core.Objects.GraphNodes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.ConnectorRevit.ViewModels
{

    public class MappingErrorViewModel : INotifyPropertyChanged
    {
        public bool HasBrokenItems => BrokenNodeSource.Count > 0;
        public ObservableCollection<NodeViewModel> BrokenNodeSource {  get; private set; }
        private Forest _brokenForest;
        public Forest BrokenForest
        {
            get => _brokenForest;
            set
            {
                _brokenForest = value;
                OnPropertyChanged(nameof(BrokenForest));
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

        public MappingErrorViewModel( )
        {
            BrokenNodeSource = new ObservableCollection<NodeViewModel>();
            MediatorFromVM.Register("BrokenForestChanged", forest => UpdateBrokenNodes((Forest)forest));
        }

        private void UpdateBrokenNodes(Forest forest)
        {
            if(forest.Trees?.Count > 0)
            {
                BrokenForest = forest;
                BrokenNodeSource.Clear();
                foreach (var tree in forest.Trees)
                {
                    BrokenNodeSource.Add(new NodeViewModel(tree));
                }
            }
            else
            {
                BrokenForest = null;
                BrokenNodeSource.Clear();
            }
            OnPropertyChanged(nameof(BrokenNodeSource));
            OnPropertyChanged(nameof(HasBrokenItems));
        }

        public void HandleMappingErrorClicked()
        {
            BrokenSectionVisibility = BrokenSectionVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
