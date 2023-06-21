using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Views;

namespace Calc.ConnectorRevit.Revit
{
    public class App : IExternalApplication
    {
        public static ViewModel ViewModel { get; set; }
        public static Document CurrentDoc { get; set; }
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonMaker ribbonMaker = new RibbonMaker();
            ribbonMaker.Create(application);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}