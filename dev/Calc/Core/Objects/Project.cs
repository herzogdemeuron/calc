using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects
{
    public class Project : IShowName
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("standard")]
        public LcaStandard Standard { get; set; }
        public string ShowName => $"{Number} - {Name}";
    }
}