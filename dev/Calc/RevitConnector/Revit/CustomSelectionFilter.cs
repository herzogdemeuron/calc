using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.RevitConnector.Revit
{
    public class CustomSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        private List<Type> _types = new List<Type>()
        {
            typeof(Autodesk.Revit.DB.Wall),
            typeof(Autodesk.Revit.DB.Floor),
            typeof(Autodesk.Revit.DB.RoofBase),
            typeof(Autodesk.Revit.DB.FamilyInstance)
        };
        public CustomSelectionFilter()
        {
        }
        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            return _types.Contains(elem.GetType());
        }
        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return false;
        }
    }
}
