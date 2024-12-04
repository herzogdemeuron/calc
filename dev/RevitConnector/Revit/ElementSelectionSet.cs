﻿using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Security.Policy;

namespace Calc.RevitConnector.Revit
{
    /// <summary>
    /// The selection result from revit, including the element ids and the paramaters of the group.
    /// </summary>
    public class ElementSelectionSet
    {
        public string RevitGroupName { get; set; }
        public GroupType RevitGroupType { get; set; }
        public string RevitGroupModel { get; set; }
        public string RevitGroupDescription { get; set; }
        public List<ElementId> ElementIds { get; set; } = new List<ElementId>();
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public void AddId(ElementId elementId)
        {
            if (!ElementIds.Contains(elementId))
            {
                ElementIds.Add(elementId);
            }
        }

        public void AddIds(List<ElementId> elementIds)
        {
            foreach (var elementId in elementIds)
            {
                AddId(elementId);
            }
        }

        public void AddParameters(List<Parameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.IsShared)
                {
                    Parameters.Add(parameter.Definition.Name, parameter.AsValueString());
                }
            }
        }
    }
}
