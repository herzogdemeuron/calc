using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Standards
{
    /// <summary>
    /// This class is exclusively used to map the junction item, from many to many relations: buildup - standard
    /// each standard item contains one LcaStandard that are related to a buildup 
    /// </summary>
    internal class StandardItem
    {
        [JsonProperty("standard")]
        public LcaStandard Standard { get; set; }
    }
}
