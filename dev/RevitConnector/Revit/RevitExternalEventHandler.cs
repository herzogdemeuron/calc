using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace Calc.RevitConnector.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class RevitExternalEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent exEvent;
        private Action ActionToExecute { get; set; }

        public RevitExternalEventHandler()
        {
            exEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                ActionToExecute?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        public void Raise(Action actionToExecute)
        {
            ActionToExecute = actionToExecute;
            exEvent.Raise();
        }

        public string GetName()
        {
            return "GeneralEventHandler";
        }
    }
}
