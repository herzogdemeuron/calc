using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects
{
    public class Project
    {
        [JsonProperty(PropertyName = "id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Id { get; set; } = -1;
        [JsonProperty(PropertyName = "project_number")]
        public string ProjectNumber { get; set; }
    }
}