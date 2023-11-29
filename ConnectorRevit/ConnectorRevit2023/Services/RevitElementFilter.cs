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
                  .Select(elem => CreateCalcElement(elem, parameterNameList, AreName:".StandardArea")).ToList();
            return collector;
        }


        static private CalcElement CreateCalcElement(
            Element elem, 
            List<string> parameterNameList,
            string LenName = "Length",
            string AreName = "Area",
            string VolName = "Volume"
            )
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
            string type = App.CurrentDoc.GetElement(typeId).Name;
            string typeName = $"{type} ({typeId.IntegerValue})";

            decimal? lenValue = GetBasicValue(elem, LenName);
            decimal? araValue = GetBasicValue(elem, AreName);
            decimal? volValue = GetBasicValue(elem, VolName);


            return new CalcElement(elem.Id.ToString(), typeName, parameterDictionary, lenValue, araValue, volValue);
        }

        /// <summary>
        /// The basic quantity parameter value of an element,
        /// if the parameter is not found, return null, this happens when the element does not contain the custom parameter (for doors and windows) or the default parameter (wrong unit setup)
        /// or if the parameter value is 0, return 0, this happens when the parameter is not assigned a value, meaning the user may have wrong setups for material unit or the element needs new custom parameter to reflect the quatity
        /// which will trigger an error in the calculation
        /// </summary>
        static private decimal? GetBasicValue(Element elem, string parameterName)
        {
            Parameter parameter = elem.LookupParameter(parameterName);
            if (parameter == null) return null;
            var value = ParameterHelper.ToMetricValue(parameter);
            return (decimal)value;
        }

    }
}
