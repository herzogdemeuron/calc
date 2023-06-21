using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.ConnectorRevit.Views;

namespace Calc.ConnectorRevit.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class StartCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                MockData.Initiate();
                App.CurrentDoc = commandData.Application.ActiveUIDocument.Document;
                App.ViewModel = new ViewModel();
                MainView mainView = new MainView();
                mainView.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Result.Failed;
            }
        }
    }
}