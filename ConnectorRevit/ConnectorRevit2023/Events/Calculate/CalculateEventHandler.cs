using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.Core.DirectusAPI;

namespace Calc.ConnectorRevit
{
    [Transaction(TransactionMode.Manual)]
    public class CalculateEventHandler : IExternalEventHandler
    {
        private readonly MainViewModel _viewModel;

        public CalculateEventHandler(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Execute(UIApplication uiapp)
        {
            Debug.WriteLine("CalculateEventHandler.Execute started");
            //Calculator calculator = new Calculator(uiapp);
            try
            {
                Debug.WriteLine("CalculateEventHandler.Execute()");
                var directus = new Directus();
                Debug.WriteLine("test" + directus.Url);
                //_viewModel.GetAllBuildups();
                //calculator.TriggerCalculation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public string GetName()
        {
            return "CalculateEventHandler";
        }
    }
}

