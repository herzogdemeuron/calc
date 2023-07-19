using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Helpers
{
    public class ParameterHelper
    {
        public static object CheckAndGet(Element element, string parameterName)
        {
            // check if the Element has the parameter
            // if not, return false
            // if yes, return the parameter value convert to metric
            // if the original parameter name is illegal, return false
            (bool instance, string name) = GetParameterName(parameterName);

            if (name == null)
            {
                return false;
            }

            Parameter parameter;

            if (instance)
            {
                parameter = element.LookupParameter(parameterName);
            }
            else
            {
                parameter = LookupTypeParameter(element, parameterName);
            }

            if (parameter == null)
            {
                return false;
            }
            if (parameter.StorageType != StorageType.Double)
            {
                return parameter.AsValueString();
            }
            return ToMetricValue(parameter);

        }

        public static (bool,string) GetParameterName(string parameterName)
        {
            // type paramter name: "type: parameterName"
            // instance parameter name: "inst: parameterName"
            // check firstly if the parameter name is legal
            // if yes, return true for instance false for type parameter, and the parameter name
            // if no, return null
            if (parameterName.StartsWith("type:"))
            {
                return (false, parameterName.Substring(6).Trim());
            }
            else if (parameterName.StartsWith("inst:"))
            {
                return (true, parameterName.Substring(6).Trim());
            }
            else
            {
                return (false, null);
            }
        }
        public static List<string> ValidateParameterNames(List<string> parameterNames)
        {
            // check if the parameter names are legal
            // if yes, return the parameter names
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
            return type.LookupParameter(parameterName);
        }
        public static double ToMetricValue(Parameter parameter, object dummyValue = null)
        {
            if (parameter == null) return 0;
           
            double value = (dummyValue != null) ? (double)dummyValue : parameter.AsDouble();
            // convert parameter value to metric if the GetDataType is length, area or volume
            ForgeTypeId typeId = parameter.Definition.GetDataType();

            if (typeId == SpecTypeId.Length)
            {
                return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
            }
            else if (typeId == SpecTypeId.Area)
            {
                return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.SquareMeters);
            }
            else if (typeId == SpecTypeId.Volume)
            {
                return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.CubicMeters);
            }
            else
            {
                return value;
            }

        }
    }
}
