using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.GraphNodes;
using Calc.MVVM.Helpers;
using Calc.MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// Wraps an IGraphNode.
    /// </summary>
    public class NodeModel : INotifyPropertyChanged
    {
        public bool IsLeftover { get => CheckLeftover(); }
        public string Name { get => GetNodeName(); }
        public string BranchParameterName { get => GetParameterName(); }
        public bool? BranchParameterIsInstance { get => CheckIfParameterIsInstance(); }
        public bool IsBranch { get => CheckIfBranch(); } // is a branch but not a query
        public bool IsBrokenNode => Host is Branch && (Host as Branch).QueryTemplate == null; // mark as broken if parent query template is null
        public NodeTreeViewModel ParentTreeView { get; set; }
        public NodeModel ParentNodeItem { get; private set; }
        public ObservableCollection<NodeModel> SubNodeItems { get; }
        public AssemblyModel AssemblyModel { get; set; }
        private IGraphNode host;
        public IGraphNode Host
        {
            get { return host; }
            set
            {
                if (host != value)
                {
                    host = value;
                    OnPropertyChanged(nameof(Host));
                }
            }
        }
        private bool _labelColorVisible;
        public bool LabelColorVisible
        {
            get => _labelColorVisible;
            set
            {
                if (_labelColorVisible != value)
                {
                    _labelColorVisible = value;
                    OnPropertyChanged(nameof(LabelColorVisible));
                }
            }
        }
        public Visibility UnderlineVisibility
        {
            get
            {
                return ParentTreeView?.BranchesSwitch == false ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public NodeModel(IGraphNode node, NodeTreeViewModel parentTreeView = null, NodeModel parentNodeItem = null)
        {
            Host = node;
            ParentTreeView = parentTreeView;
            SubNodeItems = new ObservableCollection<NodeModel>();
            AssemblyModel = new AssemblyModel(this);
            ParentNodeItem = parentNodeItem;
            if(node == null) return;
            foreach (var subNode in node.SubBranches)
            {
                SubNodeItems.Add(new NodeModel(subNode, parentTreeView, this));
            }
        }
        
        internal void SetAssembly(bool setMain, Assembly assembly)
        {
            if (setMain)
            {
                AssemblyModel.Assembly1 = assembly;
            }
            else
            {
                AssemblyModel.Assembly2 = assembly;
            }
        }

        /// <summary>
        /// Removes the branch node from its parent node.
        /// </summary>
        /// <returns>The next node to select.</returns>
        internal NodeModel RemoveFromParent()
        {
            if (ParentNodeItem == null || IsBranch == false)
                return null;
            var index = ParentNodeItem.SubNodeItems.IndexOf(this);
            ParentNodeItem.SubNodeItems.RemoveAt(index);
            ParentNodeItem.Host.SubBranches.RemoveAt(index);
            if (index < ParentNodeItem.SubNodeItems.Count)
            {
                return ParentNodeItem.SubNodeItems[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Refreshes the assembly ui section of the node and all its subnodes
        /// </summary>
        internal void RefreshAssemblySection()
        {
            AssemblyModel.RefreshAssemblySection();
            if (SubNodeItems.Count > 0)
            {
                foreach (var subNode in SubNodeItems)
                {
                    subNode.RefreshAssemblySection();
                }
            }
        }

        private bool CheckLeftover()
        {
            if (Host is QueryTemplate qryTemplate)
                return qryTemplate.IsLeftover;
            else
                return ParentNodeItem.IsLeftover;
        }

        private string GetNodeName()
        {
            if (Host is Query query)
                return query.Name;
            else if (Host is Branch branch)
                return branch.Value;
            else if (Host is QueryTemplate qryTemplate)
                return qryTemplate.Name;
            else
                return "Please select a query template";
        }

        private string GetParameterName()
        {
            if (Host is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item2;
            else
                return null;
        }

        /// <summary>
        /// Checks if the parameter is an instance parameter.
        /// </summary>
        private bool? CheckIfParameterIsInstance()
        {
            if (Host is Branch branch)
                return ParameterHelper.GetParameterInfo(branch.Parameter).Item1;
            else
                return null;
        }

        /// <summary>
        /// Checks if the node is a branch but not a query.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfBranch()
        {
            return Host is Branch && !(Host is Query);
        }

        /// <summary>
        /// Notify all the properties changed of the node and all its subnodes.
        /// </summary>
        internal void NotifyNodePropertyChange()
        {
            OnPropertyChanged(nameof(UnderlineVisibility));
            OnPropertyChanged(nameof(LabelColorVisible));
            OnPropertyChanged(nameof(Host));
            foreach (var subBranch in SubNodeItems)
            {
                subBranch.NotifyNodePropertyChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
