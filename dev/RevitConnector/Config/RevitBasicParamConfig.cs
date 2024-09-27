using Autodesk.Revit.DB;

namespace Calc.RevitConnector.Config
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
        public string CountName { get; private set; } = "count";

        public RevitBasicParamConfig(
            BuiltInCategory category = BuiltInCategory.INVALID,
            string LengthName = null, 
            string AreaName = null,
            string VolumeName = null
            )
        {
            this.category = category;
            this.LengthName = string.IsNullOrEmpty(LengthName) ? "Length" : LengthName;
            this.AreaName = string.IsNullOrEmpty(AreaName) ? "Area" : AreaName;
            this.VolumeName = string.IsNullOrEmpty(VolumeName) ? "Volume" : VolumeName;
        }
    }
}
