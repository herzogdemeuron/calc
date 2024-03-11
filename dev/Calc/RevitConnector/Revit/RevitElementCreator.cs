using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.RevitConnector.Config;
using Calc.RevitConnector.Helpers;
using Calc.Core.Objects;
using Calc.Core.Objects.Results;
using Calc.Core.Interfaces;
using Calc.Core.Objects.BasicParameters;

namespace Calc.RevitConnector.Revit
{
    public class RevitElementCreator : IElementCreator

    {
        /// <summary>
        /// Create a list of calc elements from the current document
        /// it only takes the relevant parameters from the parameter name list to speed up the process
        /// </summary>
        
        private readonly Document doc;
        
        public RevitElementCreator(Document doc)
        {
            this.doc = doc;
        }
        public List<CalcElement> CreateCalcElements(List<string> parameterNameList)
        {
            var doorParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Doors, AreaName: ".Standard_Area");
            var windowParamConfig = new RevitBasicParamConfig(BuiltInCategory.OST_Windows, AreaName: ".Area");
            var paramConfigs = new List<RevitBasicParamConfig> { doorParamConfig, windowParamConfig };
            List<CalcElement> result = new List<CalcElement>();

            var collector = new FilteredElementCollector(doc)
                  .WhereElementIsNotElementType()
                  .WhereElementIsViewIndependent()
                  .Where(x =>
                  x.Category != null &&
                  x.GetTypeId() != null &&
                  x.GetTypeId() != ElementId.InvalidElementId).ToList();

            foreach (RevitBasicParamConfig paramConfig in paramConfigs)
            {

                result.AddRange
                    (
                        CalcElementsFromParamConfig(collector, parameterNameList, paramConfig)
                    );
            }

            result.AddRange
                (
                    CalcElementsFromParamConfig(collector, parameterNameList, new RevitBasicParamConfig())
                );

            return result;
        }

        private List<CalcElement> CalcElementsFromParamConfig
            (
            List<Element> elementList,
            List<string> parameterNameList,
            RevitBasicParamConfig paramConfig
            )
        {
            var result = new List<CalcElement>();

            var builtinCategory = paramConfig.category;
            var lengthName = paramConfig.LengthName;
            var areaName = paramConfig.AreaName;
            var volumeName = paramConfig.VolumeName;

            //takes the whole list if the category is invalid, otherwise filter the list by category
            var filteredElements = (int)builtinCategory == -1 ?
                elementList :
                elementList.Where(x => x.Category.Id.IntegerValue == (int)builtinCategory);

            result = filteredElements.Select
                (
                x => CreateCalcElement(x, parameterNameList, lengthName, areaName, volumeName)
                ).ToList();

            // remove the result elements from the elementList
            elementList.RemoveAll(x => x.Category.Id.IntegerValue == (int)builtinCategory);
            return result;
        }

        /// <summary>
        /// create a calc element using the element and the parameter name list
        /// add as many parameters from the list as possible
        /// </summary>
        private CalcElement CreateCalcElement(
            Element elem,
            List<string> parameterNameList,
            string lenName,
            string areaName,
            string volName
            )
        {
            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();
            foreach (string parameterName in parameterNameList)
            {
                Tuple<bool, object> result = ParameterHelper.CheckAndGet(elem, parameterName);
                if (result.Item1 == false) continue;
                parameterDictionary[parameterName] = result.Item2;
            }

            ElementId typeId = elem.GetTypeId();
            string type = doc.GetElement(typeId).Name;
            string typeName = $"{type} ({typeId.IntegerValue})";

            var lenParam = ParameterHelper.CreateBasicUnitParameter(elem, lenName, Unit.m);
            var areaParam = ParameterHelper.CreateBasicUnitParameter(elem, areaName, Unit.m2);
            var volParam = ParameterHelper.CreateBasicUnitParameter(elem, volName, Unit.m3);


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
    }
}
