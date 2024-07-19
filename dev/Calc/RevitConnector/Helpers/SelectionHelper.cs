﻿using Autodesk.Revit.DB;
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
        public static ElementSelectionSet SelectElements(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            var filter = new CustomSelectionFilter();
            try
            {
                var references = selection.PickObjects(
                    Autodesk.Revit.UI.Selection.ObjectType.Element,
                    filter,
                    "Select Elements").ToList();
                return GetElementSelectionSet(references, uidoc);
            }
            catch (Exception)
            {
                TaskDialog.Show("Error", "Error occured while selecting elements");
                return new ElementSelectionSet();
            }
        }

        /// <summary>
        /// filter elements, get the element selection set
        /// the elements are elements inside the groups and the outsiders
        /// if there is only one group selected, extract the parameters from it, store it into the element selection set
        /// </summary>
        private static ElementSelectionSet GetElementSelectionSet(List<Reference> references, UIDocument uidoc)
        {
            var selectionSet = new ElementSelectionSet();
            var selectedGroups = new List<Element>();

            // collect outsiders and groups
            foreach (var reference in references)
            {
                var element = uidoc.Document.GetElement(reference);
                if (element is Group)
                {
                    selectedGroups.Add(element);
                }
                else
                {
                    selectionSet.AddId(element.Id);
                }
            }

            // add elements from the groups that pass customSelectionFilter to selectionSet
            // use element filter to exclude model groups
            var customSelectionFilter = new CustomSelectionFilter();
            var elementFilter = new ElementCategoryFilter(BuiltInCategory.OST_IOSModelGroups, true);
            foreach (var group in selectedGroups)
            {
                var groupElementIds = group
                    .GetDependentElements(elementFilter)
                    .Where
                    (x =>
                    customSelectionFilter.AllowElement(uidoc.Document.GetElement(x))
                    ).ToList();

                selectionSet.AddIds(groupElementIds);
            }

            // if there is only one group selected, extract the parameters from it
            if (selectedGroups.Count == 1)
            {
                var group = selectedGroups.First();
                selectionSet.RevitGroupTypeId = group.GetTypeId().IntegerValue;
                var parameters = group.Parameters.Cast<Parameter>().ToList();
                selectionSet.AddParameters(parameters);
                selectionSet.RevitGroupName = group.Name;
            }

            return selectionSet;

        }
        
    }
}

   
