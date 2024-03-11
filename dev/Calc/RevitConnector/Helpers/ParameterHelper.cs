using Autodesk.Revit.DB;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc.RevitConnector.Helpers
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
                value = ToMetricValue(value, Unit.m);
            }
            else if (typeId == SpecTypeId.Area)
            {
                value = ToMetricValue(value, Unit.m2);
            }
            else if (typeId == SpecTypeId.Volume)
            {
                value = ToMetricValue(value, Unit.m3);
            }

            return Math.Round(value, 5);            
        }

        public static double ToMetricValue(double value, Unit unit)
        {
            switch (unit)
            {
                case Unit.m:
                    return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
                case Unit.m2:
                    return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.SquareMeters);
                case Unit.m3:
                    return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.CubicMeters);
                default:
                    return value;
            }
        }



        /// <summary>
        /// get the basic unit parameter of an element
        /// create a basic unit parameter with error type if the parameter is missing or redundant
        /// </summary>
        public static BasicParameter CreateBasicUnitParameter(Element elem, string parameterName, Unit unit)
        {
            if (unit == Unit.piece)
            {
                return new BasicParameter()
                {
                    Name = parameterName,
                    Amount = 1,
                    Unit = unit
                };
            }

            var parameters = elem.GetParameters(parameterName);
            if (parameters.Count == 0)
            {
                return new BasicParameter()
                {
                    Name = parameterName,
                    ErrorType = ParameterErrorType.Missing,
                    Unit = unit
                };
            }
            else if (parameters.Count > 1)
            {
                return new BasicParameter()
                {
                    Name = parameterName,
                    ErrorType = ParameterErrorType.Redundant,
                    Unit = unit
                };
            }

            var value = ParameterHelper.ToMetricValue(parameters.First());

            if (value == 0)
            {
                return new BasicParameter()
                {
                    Name = parameterName,
                    ErrorType = ParameterErrorType.ZeroValue,
                    Unit = unit
                };
            }
            else
            {
                return new BasicParameter()
                {
                    Name = parameterName,
                    Amount = (double)value,
                    Unit = unit
                };
            }
        }
    }
}
