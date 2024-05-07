using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Calc.RevitConnector.Revit
{
    public class CustomSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        private List<Type> allowedTypes = new List<Type>()
        {
            typeof(Wall),
            typeof(Floor),
            typeof(Ceiling),
            typeof(RoofBase),
            typeof(FamilyInstance),
            typeof(Mullion),
            typeof(Panel),
            typeof(ModelCurve)
        };

        public CustomSelectionFilter()
        {
        }

        private bool CheckCurtainWall(Element elem)
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

        public bool AllowElement(Element elem)
        {
            if (CheckCurtainWall(elem))
            {
                return false;
            }
            return allowedTypes.Contains(elem.GetType());
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
