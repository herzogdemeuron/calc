using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    public class Calculator
    {
        private readonly Document Doc;
        public Branch SelectedBranch { get; set; }

        public Calculator(UIApplication uiapp)
        {
            Doc = uiapp.ActiveUIDocument.Document;
        }

        public void TriggerCalculation()
        {             
            Debug.WriteLine("Calculation Triggered");
        }
    }
}
