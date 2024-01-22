using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BrokenNodesViewModel : INotifyPropertyChanged
    {
        private Forest currentBrokenForest;
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
        public BrokenNodesViewModel(NodeTreeViewModel NodeTreeVM)
        {
            BrokenNodeSource = new ObservableCollection<NodeViewModel> { NodeTreeVM.CurrentBrokenForestItem };
        }

        private void UpdateBrokenNodes(Forest forest)
        {
            currentBrokenForest = forest;
            OnPropertyChanged(nameof(BrokenNodeSource));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
