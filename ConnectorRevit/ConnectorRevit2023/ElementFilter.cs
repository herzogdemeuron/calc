using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    public class ElementFilter
        //filters revit objtcts in revit using roots
        //and outputs a list of calc elements
    {
        static public List<CalcElement> GetCalcElements(Document doc, List<Root> roots)
        {
            IEnumerable<Element> collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent();
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
            return new CalcElement(elem.Id.ToString(), parameterDictionary);
        }

    }
}
