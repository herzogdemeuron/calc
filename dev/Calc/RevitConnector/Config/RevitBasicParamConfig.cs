using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.MVVM.Config
{
    /// <summary>
    /// defines the basic parameter names (len, area and vol) for multiple categories that are used for calc element creation
    /// </summary>
    public class RevitBasicParamConfig
    {
        public BuiltInCategory category { get; private set; }
        public string LengthName { get; private set; }
        public string AreaName { get; private set; }
        public string VolumeName { get; private set; }

        public RevitBasicParamConfig(
            BuiltInCategory category = BuiltInCategory.INVALID, 
            string LengthName = "Length", 
            string AreaName = "Area",
            string VolumeName = "Volume"
            )
        {
            this.category = category;
            this.LengthName = LengthName;
            this.AreaName = AreaName;
            this.VolumeName = VolumeName;
        }
    }
}
