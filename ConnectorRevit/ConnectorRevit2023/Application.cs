using Autodesk.Revit.UI;

namespace Calc.ConnectorRevit
{
    public class Application : IExternalApplication
    {
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