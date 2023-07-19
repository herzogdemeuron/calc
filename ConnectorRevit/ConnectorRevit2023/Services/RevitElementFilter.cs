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
            /*Element elem = App.CurrentDoc.GetElement(new ElementId(767506));
            return new List<CalcElement>()
            {
                CreateCalcElement(elem, parameterNameList)
            };*/
            List<CalcElement> collector = new FilteredElementCollector(App.CurrentDoc)
                  .WhereElementIsNotElementType()
                  .WhereElementIsViewIndependent()
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

            decimal _area = (decimal)GetBasicValue(elem, "Area");
            decimal _volume = (decimal)GetBasicValue(elem, "Volume");
            decimal _length = (decimal)GetBasicValue(elem, "Length");

            return new CalcElement(elem.Id.ToString(), parameterDictionary, _length, _area, _volume);
        }

        static private double GetBasicValue(Element elem, string parameterName)
        {
            Parameter parameter = elem.LookupParameter(parameterName);
            return ParameterHelper.ToMetricValue(parameter);

        }

        /*        static private double ConvertToMetric(double value, string unitType)
                {
                    switch (unitType)
                    {
                        case "Length":
                            return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
                        case "Area":
                            return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.SquareMeters);
                        case "Volume":
                            return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.CubicMeters);
                        default:
                            return value;
                    }
                }*/

        /*        static public List<CalcElement> GetCalcElements(Tree tree)
        {
            //create calc elements using only the necessary parameters of the forest
            IEnumerable<Element> collector = new FilteredElementCollector(App.CurrentDoc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent();
            List<Root> roots = tree.Roots;
            foreach (Root root in roots)
            {
                collector = collector.Where(e => CheckContains(e, root.Parameter, root.Value));
            }
            List<CalcElement> calcElements = collector.Select(CreateCalcElement).ToList();
            return calcElements;
        }

        static private bool CheckContains(Element element, string parameter, string value)
        {
            Parameter param = element.LookupParameter(parameter);
            return param?.AsValueString()?.Contains(value) ?? false;
        }*/

    }
}
