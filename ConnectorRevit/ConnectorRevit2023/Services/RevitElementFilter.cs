using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Config;
using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using Calc.Core.Objects.Results;

namespace Calc.ConnectorRevit.Services
{
    public class RevitElementFilter

    {
        /// <summary>
        /// Create a list of calc elements from the current document
        /// it only takes the relevant parameters from the parameter name list to speed up the process
        /// </summary>
        static public List<CalcElement> CreateCalcElements(List<string> parameterNameList, params RevitBasicParamConfig[] paramConfigs)
        {
            /* Element elem = App.CurrentDoc.GetElement(new ElementId(767264));
             return new List<CalcElement>()
             {
                 CreateCalcElement(elem, parameterNameList)
             };*/
            List<CalcElement> result = new List<CalcElement>();

            var collector = new FilteredElementCollector(App.CurrentDoc)
                  .WhereElementIsNotElementType()
                  .WhereElementIsViewIndependent()
                  .Where(x => (x.Category != null) && x.GetTypeId() != null && x.GetTypeId() != ElementId.InvalidElementId).ToList();

            foreach(RevitBasicParamConfig paramConfig in paramConfigs)
            {
                
                result.AddRange(CalcElementsFromParamConfig(collector, parameterNameList, paramConfig));
            }

            // add the rest of the elements
            result.AddRange(CalcElementsFromParamConfig(collector, parameterNameList, new RevitBasicParamConfig()));

            return result;
        }

        static private List<CalcElement> CalcElementsFromParamConfig(List<Element> elementList, List<string> parameterNameList, RevitBasicParamConfig paramConfig)
        {
            var result = new List<CalcElement>();

            var builtinCategory = paramConfig.category;
            var lengthName = paramConfig.LengthName;
            var areaName = paramConfig.AreaName;
            var volumeName = paramConfig.VolumeName;

            //takes the whole list if the category is invalid, otherwise filter the list by category
            var filteredElements = (int)builtinCategory == -1 ? elementList : elementList.Where(x => x.Category.Id.IntegerValue == (int)builtinCategory);
            result = filteredElements.Select(x => CreateCalcElement(x, parameterNameList, lengthName, areaName, volumeName)).ToList();
            // remove the result elements from the elementList
            elementList.RemoveAll(x => x.Category.Id.IntegerValue == (int)builtinCategory);
            return result;
        }

        static private CalcElement CreateCalcElement(
            Element elem, 
            List<string> parameterNameList,
            string lenName,
            string areaName,
            string volName
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

            var lenParam = CreateBasicUnitParameter(elem, lenName, Unit.m);
            var areaParam = CreateBasicUnitParameter(elem, areaName, Unit.m2);
            var volParam = CreateBasicUnitParameter(elem, volName, Unit.m3);


            return new CalcElement
                (
                elem.Id.ToString(), 
                typeName, 
                parameterDictionary, 
                lenParam, 
                areaParam, 
                volParam
                );
        }

        /// <summary>
        /// get the basic unit parameter of an element
        /// create a basic unit parameter with error type if the parameter is missing or redundant
        /// </summary>
        static private BasicUnitParameter CreateBasicUnitParameter(Element elem, string parameterName, Unit unit)
        {
            var parameters = elem.GetParameters(parameterName);
            if (parameters.Count == 0)
            {
                return new BasicUnitParameter() 
                { 
                    Name = parameterName, 
                    ErrorType = ParameterErrorType.Missing, 
                    Unit = unit 
                };
            }
            else if (parameters.Count > 1) 
            {
                return new BasicUnitParameter() 
                { 
                    Name = parameterName, 
                    ErrorType = ParameterErrorType.Redundant, 
                    Unit = unit 
                };
            }

            var value = ParameterHelper.ToMetricValue(parameters.First());

            if (value == 0)
            {
                return new BasicUnitParameter() 
                { 
                    Name = parameterName, 
                    ErrorType = ParameterErrorType.ZeroValue, 
                    Unit = unit 
                };
            }
            else
            {
                return new BasicUnitParameter() 
                { 
                    Name = parameterName, 
                    Value = (decimal)value, 
                    Unit = unit 
                };
            }
        }

    }
}
