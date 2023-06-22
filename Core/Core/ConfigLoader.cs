using System.IO;
using Speckle.Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.Core
{
    public class ConfigLoader
    {
        public static Dictionary<string, string> Load(string configPath="")
        {
            // check if config path is not empty sting
            if (configPath == "")
            {
                configPath = "C:/Program Files/Autodesk/Revit 2023/AddIns/CalcRevit/config.json";
            }
            // check if config path is valid
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Config file not found at " + configPath);
            }
            var configFile = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(configFile);
        }
    }
}
