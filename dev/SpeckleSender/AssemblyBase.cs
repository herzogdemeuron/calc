using Calc.Core.Objects.Assemblies;
using Speckle.Core.Models;
using System.Collections.Generic;

namespace SpeckleSender
{
    internal class AssemblyBase : Base
    {
        public string assembly_code { get; set; }
        public string assembly_name { get; set; }
        public string assembly_group { get; set; }
        public string description { get; set; }
        public List<Base> elements { get; set; }

        public AssemblyBase(AssemblyData assemblyData, List<Base> elementBases)
        {
            assembly_code = assemblyData.Code;
            assembly_name = assemblyData.Name;
            assembly_group = assemblyData.Group;
            description = assemblyData.Description;
            elements = elementBases;
        }
    }
}
