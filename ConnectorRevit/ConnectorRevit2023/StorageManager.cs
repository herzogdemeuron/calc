using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Polly;
using GraphQL.Client.Http;
using Autodesk.Revit.UI;
using Calc.Core;

namespace Calc.ConnectorRevit
{
    public class StorageManager
    {


        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }

    }
}
