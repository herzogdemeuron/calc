using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.RevitConnector.Revit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc.RevitConnector.Helpers
{
    /// <summary>
    /// Prompts user to select elements in revit with some specific catetgories using ISelectionFilter.
    /// </summary>
    internal static class SelectionHelper
    {
        public static List<ElementSelectionSet> SelectElements(UIDocument uidoc)
        {
            var selection = uidoc.Selection;
            var filter = new CustomSelectionFilter();
            try
            {
                var references = selection.PickObjects(
                    Autodesk.Revit.UI.Selection.ObjectType.Element,
                    filter,
                    "Select Elements").ToList();
                return GetElementSelectionSets(references, uidoc);
            }
            catch (Exception)
            {
                return new List<ElementSelectionSet>();
            }
        }

        /// <summary>
        /// Gets the element selection set from a group.
        /// </summary>
        private static ElementSelectionSet GetSelectionSetFromGroup(Group group, UIDocument uidoc)
        {
            var selectionSet = new ElementSelectionSet();
            // add elements from the groups that pass customSelectionFilter to selectionSet
            var customSelectionFilter = new CustomSelectionFilter();
            var elementFilter = new ElementCategoryFilter(BuiltInCategory.OST_IOSModelGroups, true);
            var groupElementIds = group
                    .GetDependentElements(elementFilter)
                    .Where
                    (x =>
                    customSelectionFilter.AllowProcessElement(uidoc.Document.GetElement(x))
                    ).ToList();
            selectionSet.AddIds(groupElementIds);
            // extract parameters from the group            
            selectionSet.RevitGroupType = uidoc.Document.GetElement(group.GetTypeId()) as GroupType;
            var parameters = group.Parameters.Cast<Parameter>().ToList();
            selectionSet.AddParameters(parameters);
            selectionSet.RevitGroupName = group.Name;
            selectionSet.RevitGroupDescription = selectionSet.RevitGroupType?.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION)?.AsString();
            selectionSet.RevitGroupModel = selectionSet.RevitGroupType?.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL)?.AsString();            
            return selectionSet;
        }

        /// <summary>
        /// Filters elements, gets the element selection sets from groups.
        /// </summary>
        private static List<ElementSelectionSet> GetElementSelectionSets(List<Reference> references, UIDocument uidoc)
        {
            var selectionSets = new List<ElementSelectionSet>();
            var selectedGroups = new List<Element>();
            // collect only groups
            foreach (var reference in references)
            {
                var element = uidoc.Document.GetElement(reference);
                if (element is Group)
                {
                    selectedGroups.Add(element);
                }                
            }
            // get selection sets from groups
            foreach (var group in selectedGroups)
            {
                selectionSets.Add(GetSelectionSetFromGroup(group as Group, uidoc));
            }
            return selectionSets;
        }
        
    }
}

   
