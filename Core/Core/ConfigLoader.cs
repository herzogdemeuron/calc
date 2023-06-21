using System.IO;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core
{
    public class ConfigLoader
    {
        public Dictionary<string, string> Load()
        {
            var configFile = File.ReadAllText("C:/Program Files/Autodesk/Revit 2023/AddIns/CalcRevit/config.json");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(configFile);
        }
    }
}
