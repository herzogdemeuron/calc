using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit.Views

{
    public class TreeViewItem
    {
        public string NameString { get; set; }
        private readonly List<Branch> branches;
        public List<TreeViewItem> Children { get; set; }
        public Branch Host { get; set; }
        
        private void SetChildren()
        {
            Children = new List<TreeViewItem>();
            if (branches == null)
            {
                return;
            }
            foreach (var branch in branches)
            {
                Children.Add(new TreeViewItem(branch));
            }
        }
        public TreeViewItem(Tree tree)
        {
            NameString = tree.Name;
            Host = tree;
            branches = tree.SubBranches;
            SetChildren();
        }

        public TreeViewItem(Branch branch)
        {
            NameString = $"{branch.Parameter}: {branch.Value} ({branch.Elements.Count})";
            Host = branch;
            branches = branch.SubBranches;
            SetChildren();
        }
    }
}