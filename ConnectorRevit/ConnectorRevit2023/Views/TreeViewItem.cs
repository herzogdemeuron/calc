using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    public class TreeViewItem
    {
        public string NameString { get; set; }
        private readonly List<Branch> _branches;
        public List<TreeViewItem> Children { get; set; }
        //the property Host could be either a Tree or a Branch
        public Branch Host { get; set; }
        

        private void SetChildren()
        {
            Debug.WriteLine("Setting Children");
            Children = new List<TreeViewItem>();
            if (_branches == null)
            {
                Debug.WriteLine("No Children");
                return;
            }
            Debug.WriteLine("Children count: " + _branches.Count);
            foreach (var branch in _branches)
            {
                Children.Add(new TreeViewItem(branch));
            }
 
        }

        public TreeViewItem(Tree tree)
        {
            // a tree instance has a name property, which is a string, this would be the name string of the treeviewitem
            NameString = tree.Name;
            Host = tree;
            tree.Buildup = MockData.AllBuildups.Last();
            Debug.WriteLine("Casting Tree Name: " + NameString);
            Debug.WriteLine(tree.Roots.First().Parameter);
            //a tree instance has a trunk property, which is a single branch
            //each Branch has a SubBranches property, which is a list of Branch
            _branches = tree.SubBranches;
            SetChildren();
        }

        public TreeViewItem(Branch branch)
        {
            // a branch instance has a Parameter property, which is a string, this would be the name string of the treeviewitem
            NameString = $"{branch.Parameter}: {branch.Value} ({branch.Elements.Count})";
            Host = branch;
            branch.Buildup = MockData.AllBuildups.First();
            Debug.WriteLine("Casting branch Name: " + NameString);
            _branches = branch.SubBranches;
            SetChildren();
        }
    }
}