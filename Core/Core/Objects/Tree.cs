using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core.Objects
{
    public class Tree : Branch
    {
        public string Name { get; set; }
        public List<Root> Roots { get; set; }
        private List<string> _branchConfig;
        public List<string> BranchConfig // List of parameters to group by
        {
            get { return _branchConfig; }
            set { _branchConfig = value.ToList(); }
        }

        public Tree(List<Root> roots)
        {
            Roots = roots;
        }

        public void Plant(List<CalcElement> searchElements)
        /// <summary>
        /// Applies all 'root' filter to create a basic tree.
        /// </summary>
        {
            // ensure roots is valid
            if (Roots == null || Roots.Count == 0)
            {
                throw new Exception("Roots must be defined");
            }
            foreach (var root in Roots)
            {
                root.Elements = searchElements;
                root.CallFilterMethod(root.Method);
            }
            // perform boolean and operation for all element ids in roots and set as elements, go step by step
            // Get the list of element ids for each root
            List<List<string>> rootElementIds = Roots.Select(r => r.ElementIds).ToList();

            // Perform a boolean AND operation to find the intersection of all element ids
            List<string> commonElementIds = rootElementIds.Aggregate((a, b) => a.Intersect(b).ToList());

            // Find the corresponding elements for the common element ids
            List<CalcElement> commonElements = searchElements.Where(e => commonElementIds.Contains(e.Id)).ToList();

            // Set the common elements as the elements of the tree
            Elements = commonElements;
        }

        public void GrowBranches()
        {
            // ensure branch config is valid
            if (BranchConfig == null || BranchConfig.Count == 0)
            {
                throw new Exception("BranchConfig must be defined");
            }
            CreateBranches(BranchConfig);
        }

        public string Serialize()
        {
            var json = new StringBuilder();
            json.Append("{");
            json.Append($"\"Name\": \"{Name}\",");
            json.Append($"\"Roots\": [{string.Join(",", Roots.Select(r => r.Serialize()))}],");
            json.Append($"\"BranchConfig\": [{string.Join(",", BranchConfig.Select(b => $"\"{b}\""))}]");
            json.Append("}");
            return json.ToString();
        }
    }
}
