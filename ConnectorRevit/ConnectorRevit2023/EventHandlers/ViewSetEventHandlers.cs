using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.Core.Objects;
using Calc.ConnectorRevit.EventHandlers;
using Calc.ConnectorRevit.Views;

namespace Calc.ConnectorRevit.EventHandlers
{
    [Transaction(TransactionMode.Manual)]
    public class ViewSetEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent exEvent;

        public ViewSetEventHandler()
        {
            exEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                Visualizer.IsolateAndColor();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Raise()
        {
            exEvent.Raise();
        }

        public string GetName()
        {
            return "VisualizeEventHandler";
        }
    }
}


