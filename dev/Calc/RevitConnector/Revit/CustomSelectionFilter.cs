using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.RevitConnector.Revit
{
    public class CustomSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        private List<Type> allowedTypes = new List<Type>()
        {
            typeof(Autodesk.Revit.DB.Wall),
            typeof(Autodesk.Revit.DB.Floor),
            typeof(Autodesk.Revit.DB.RoofBase),
            typeof(Autodesk.Revit.DB.FamilyInstance),
            typeof(Autodesk.Revit.DB.Mullion),
            typeof(Autodesk.Revit.DB.Panel),
            typeof(Autodesk.Revit.DB.ModelCurve)
        };

        public CustomSelectionFilter()
        {
        }
        private bool checkCurtainWall(Element elem)
        {
            if (elem.GetType() == typeof(Wall))
            {
               var wall = elem as Wall;
                if (wall.WallType.Kind == WallKind.Curtain)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            if (checkCurtainWall(elem))
            {
                return false;
            }
            return allowedTypes.Contains(elem.GetType());
        }
        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return false;
        }
    }
}
