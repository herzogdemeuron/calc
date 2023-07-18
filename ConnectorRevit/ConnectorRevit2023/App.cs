using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.ConnectorRevit.Revit;
using Calc.ConnectorRevit.Views;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core.DirectusAPI;
using Calc.Core;

namespace Calc.ConnectorRevit
{
    public class App : IExternalApplication
    {
        //public static ViewModelDepot ViewModel { get; set; }
        public static Document CurrentDoc { get; set; }
        public static string RevitVersion { get; set; }
        //public static DirectusStore Store { get; set; }

        public static ExternalEventHandler EventHandler;
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonMaker.Create(application);
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}