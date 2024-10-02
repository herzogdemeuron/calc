using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// wraps the assembly data fo the element sender
    /// </summary>
    public class AssemblyData
    {
        public List<int> ElementIds { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string ModelPath { get => $"{Group}/{Code}".Replace(",","").ToLower(); }
        public Dictionary<string, string> Properties { get; set; }
    }
}
