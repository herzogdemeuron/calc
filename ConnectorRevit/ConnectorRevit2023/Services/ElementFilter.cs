using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit.Services
{
    public class ElementFilter
    //filters revit objtcts in revit using roots
    //and outputs a list of calc elements
    {
        static public List<CalcElement> GetCalcElements(Tree tree)
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
        }

        static private CalcElement CreateCalcElement(Element elem)
        {
            var parameterDictionary = elem.Parameters
                .Cast<Parameter>()
                .GroupBy(parameter => parameter.Definition.Name)
                .ToDictionary(group => group.Key, group => group.First().AsValueString() as object);

            decimal _area = (decimal)GetBasicValue(elem, "Area");
            decimal _volume = (decimal)GetBasicValue(elem, "Volume");
            decimal _length = (decimal)GetBasicValue(elem, "Length");

            return new CalcElement(elem.Id.ToString(), parameterDictionary, _length, _area, _volume);
        }

        static private double GetBasicValue(Element elem, string parameterName)
        {
            double parameterValue = elem.LookupParameter(parameterName)?.AsDouble() ?? 0;
            return ConvertToMetric(parameterValue, parameterName);
        }

        static private double ConvertToMetric(double value, string unitType)
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
        }

    }
}
