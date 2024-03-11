using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.RevitConnector.Revit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.RevitConnector.Helpers
{
    /// <summary>
    /// prompts user to select elements in revit with some specific catetgories using ISelectionFilter
    /// </summary>
    public static class SelectionHelper
    {
        public static List<ElementId> SelectElements(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            var filter = new CustomSelectionFilter();
            var elements = selection.PickObjects(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                filter,
                "Select Elements");
            return elements.Select(x => x.ElementId).ToList();
        }
    }
}
