using System.Collections.Generic;

namespace Calc.Core.Objects.Assemblies
{
    /// <summary>
    /// Wraps the assembly data, used by the element sender.
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
