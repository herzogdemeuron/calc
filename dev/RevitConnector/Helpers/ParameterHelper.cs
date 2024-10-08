using Autodesk.Revit.DB;
using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Elements;
using Calc.RevitConnector.Config;
using System;
using System.Linq;

namespace Calc.RevitConnector.Helpers
{
    internal class ParameterHelper
    {
        /// <summary>
        /// Gets parameter value from element, if not exist, return false.
        /// </summary>
        /// <returns>bool: if parameter exists, object: parameter value</returns>
        internal static Tuple<bool, object> CheckAndGet(Element element, string parameterName)
        {
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

        /// <summary>
        /// Gets revit parameter from element with the given name.
        /// </summary>
        private static Parameter GetParameter(Element element, string parameterName)
        {
            Tuple<bool, string> result  = GetParameterInfo(parameterName);
            bool isInstance = result.Item1;
            string name = result.Item2;            
            if (name == null) return null;
            return isInstance ? element.LookupParameter(name) : LookupTypeParameter(element, name);
        }

        /// <summary>
        /// Checks if the parameter name is legal,
        /// type paramter name with prefix "type:"
        /// instance parameter name with prefix "inst:"
        /// </summary>
        /// <returns> if legal, returns true for instance false for type parameter, and the parameter name as string.
        /// if illegal, return null</returns>
        internal static Tuple<bool, string> GetParameterInfo(string parameterName)
        {
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

        /// <summary>
        /// Gets the type parameter of an element with the given name.
        /// </summary>
        private static Parameter LookupTypeParameter(Element element, string parameterName)
        {
            ElementId id = element.GetTypeId();
            Element type = element.Document.GetElement(id);
            Parameter param =  type?.LookupParameter(parameterName) ?? null;
            return param;
        }

        /// <summary>
        /// Converts parameter value to metric if the GetDataType is length, area or volume.
        /// </summary>
        private static double ToMetricValue(Parameter parameter)
        {
            double value = parameter.AsDouble();
            // 
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

        /// <summary>
        /// Converts value from revit units to metric
        /// </summary>
        internal static double ToMetricValue(double value, Unit unit)
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
        /// Gets the basic unit parameter of an element,
        /// creates a basic unit parameter with error type if the parameter is missing or redundant.
        /// </summary>
        internal static BasicParameter CreateBasicUnitParameter(Element elem, string parameterName, Unit unit)
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
            // get parameters with unique ids
            parameters = parameters.GroupBy(p => p.Id.IntegerValue).Select(g => g.First()).ToList();
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
                    Amount = 0,
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

        /// <summary>
        /// Gets the material amount parameter of an element, either area or volume.
        /// </summary>
        internal static BasicParameter CreateMaterialAmountParameter(Element elem, ElementId materialId, Unit unit)
        {
            double amount;
            switch (unit)
            {
                case Unit.m2:
                    amount = elem.GetMaterialArea(materialId, false);
                    break;
                case Unit.m3:
                    amount = elem.GetMaterialVolume(materialId);
                    break;
                default:
                    throw new Exception($"Unit not recognized: {unit}");
            }
            if(amount == 0)
            {
                return new BasicParameter()
                {
                    Name = "Material Amount",
                    ErrorType = ParameterErrorType.ZeroValue,
                    Amount = 0,
                    Unit = unit
                };
            }
            else
            {
                var value = ToMetricValue(amount, unit);
                return new BasicParameter()
                {
                    Name = "Material Amount",
                    Amount = value,
                    Unit = unit
                };
            }          
        }

        /// <summary>
        /// Gets the revit basic param config from the calc core custom param settings,
        /// returns null if the category is invalid.
        /// </summary>
        internal static RevitBasicParamConfig ParseFromParamSetting(CustomParamSetting paramSetting)
        {
            BuiltInCategory category;
            try
            {
                category = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), paramSetting.Category);
            }
            catch
            {
                return null;
            }
            var lengthName = paramSetting.LengthCustomParamName;
            var areaName = paramSetting.AreaCustomParamName;
            var volumeName = paramSetting.VolumeCustomParamName;
            return new RevitBasicParamConfig(category, lengthName, areaName, volumeName);
        }
    }
}
