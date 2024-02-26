using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Calc.MVVM.Helpers
{
    public class ParameterHelper
    {
        public static Tuple<bool, object> CheckAndGet(Element element, string parameterName)
        {
            // bool: if parameter exists
            // object: parameter value
            var parameter = GetParameter(element, parameterName);
            if (parameter == null)
            {
                return Tuple.Create(false, (object)null);
            }

            if (parameter.StorageType != StorageType.Double)
            {
                return Tuple.Create(true, (object)parameter.AsValueString());
            }
            return Tuple.Create(true, (object)ToMetricValue(parameter));

        }

        private static Parameter GetParameter(Element element, string parameterName)
        {
            Tuple<bool, string> result  = GetParameterInfo(parameterName);
            bool isInstance = result.Item1;
            string name = result.Item2;
            
            if (name == null)
            {
                return null;
            }

            return isInstance ? element.LookupParameter(name) : LookupTypeParameter(element, name);
        }

        public static Tuple<bool, string> GetParameterInfo(string parameterName)
        {
            // type paramter name: "type: parameterName"
            // instance parameter name: "inst: parameterName"
            // check if the parameter name is legal
            // if yes, return true for instance false for type parameter, and the parameter name
            // if no, return null
            if (parameterName.StartsWith("type:"))
            {
                return Tuple.Create(false, parameterName.Substring(5).Trim());
            }
            else if (parameterName.StartsWith("inst:"))
            {
                return Tuple.Create(true, parameterName.Substring(5).Trim());
            }
            else
            {
                return Tuple.Create(false, (string)null);
            }
        }
        public static List<string> ValidateParameterNames(List<string> parameterNames)
        {
            // check if the parameter names are legal
            // if yes, return the parameter names
            // this is used before creating the calc elements
            List<string> checkedParameterNames = new List<string>();
            foreach (string parameterName in parameterNames)
            {
                if (parameterName.StartsWith("type:") || parameterName.StartsWith("inst:"))
                {
                    checkedParameterNames.Add(parameterName);
                }
            }
            return checkedParameterNames;
        }


        private static Parameter LookupTypeParameter(Element element, string parameterName)
        {
            ElementId id = element.GetTypeId();
            Element type = element.Document.GetElement(id);
            Parameter param =  type?.LookupParameter(parameterName) ?? null;
            return param;
        }
        

        public static double ToMetricValue(Parameter parameter)
        {
            double value = parameter.AsDouble();
            // convert parameter value to metric if the GetDataType is length, area or volume
            ForgeTypeId typeId = parameter.Definition.GetDataType();

            if (typeId == SpecTypeId.Length)
            {
                value = UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
            }
            else if (typeId == SpecTypeId.Area)
            {
                value = UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.SquareMeters);
            }
            else if (typeId == SpecTypeId.Volume)
            {
                value = UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.CubicMeters);
            }

           
            return Math.Round(value, 5);            

        }
    }
}
