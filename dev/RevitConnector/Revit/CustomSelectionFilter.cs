using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Calc.RevitConnector.Revit
{
    /// <summary>
    /// The selection filter, excluding WallKind.Curtain.
    /// Only group selection allowed.
    /// </summary>
    public class CustomSelectionFilter : Autodesk.Revit.UI.Selection.ISelectionFilter
    {
        private readonly List<Type> allowedSelectionTypes = new List<Type>()
        {
            typeof(Group)
        };

        private readonly List<Type> allowedProcessTypes = new List<Type>()
        {
            typeof(Wall),
            typeof(Floor),
            typeof(Ceiling),
            typeof(RoofBase),
            typeof(FamilyInstance),
            typeof(Mullion),
            typeof(Panel),
            typeof(ModelCurve),
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
            return allowedSelectionTypes.Contains(elem.GetType());
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }

        public bool AllowProcessElement(Element elem)
        {
            if (CheckCurtainWall(elem))
            {
                return false;
            }
            return allowedProcessTypes.Contains(elem.GetType());
        }
    }
}
