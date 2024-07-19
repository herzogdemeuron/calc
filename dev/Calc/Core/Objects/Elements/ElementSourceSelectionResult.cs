using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Elements
{
    public class ElementSourceSelectionResult
    {
        public string GroupName { get; set; }
        public BuildupRecord BuildupRecord { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public List<BuildupComponent> BuildupComponents { get; set; }

    }
}
