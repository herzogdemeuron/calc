using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit.Services
{
    public class RevitElementFilter

    {
        static public List<CalcElement> CreateCalcElements(List<string> parameterNameList)
        {
            /* Element elem = App.CurrentDoc.GetElement(new ElementId(767264));
             return new List<CalcElement>()
             {
                 CreateCalcElement(elem, parameterNameList)
             };*/
            List<CalcElement> collector = new FilteredElementCollector(App.CurrentDoc)
                  .WhereElementIsNotElementType()
                  .WhereElementIsViewIndependent()
                  .Where(x => (x.Category != null) && x.GetTypeId() != null && x.GetTypeId() != ElementId.InvalidElementId)
                  .Select(elem => CreateCalcElement(elem, parameterNameList)).ToList();
            return collector;
        }


        static private CalcElement CreateCalcElement(Element elem, List<string> parameterNameList)
        {
            // create a calc element using the element and the parameter name list
            // add as many parameters from the list as possible
            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();
            foreach (string parameterName in parameterNameList)
            {
                Tuple<bool, object> result = ParameterHelper.CheckAndGet(elem, parameterName);
                if (result.Item1 == false) continue;
                parameterDictionary[parameterName] = result.Item2;
            }

            ElementId typeId = elem.GetTypeId();
            string typeName = App.CurrentDoc.GetElement(typeId).Name;
            decimal _area = (decimal)GetBasicValue(elem, "Area");
            decimal _volume = (decimal)GetBasicValue(elem, "Volume");
            decimal _length = (decimal)GetBasicValue(elem, "Length");

            return new CalcElement(elem.Id.ToString(), typeName, parameterDictionary, _length, _area, _volume);
        }

        static private double GetBasicValue(Element elem, string parameterName)
        {
            Parameter parameter = elem.LookupParameter(parameterName);
            return ParameterHelper.ToMetricValue(parameter);
        }

    }
}
