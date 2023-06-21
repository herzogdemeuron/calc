using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    public class ElementFilter
        //filters revit objtcts in revit using roots
        //and outputs a list of directuslca elements
    {
        static public List<CalcElement> GetElements(Document doc, List<Root> roots)
        {
            List<CalcElement> elements = new List<CalcElement>();

            IEnumerable<Element> collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().WhereElementIsViewIndependent();
            foreach (Root root in roots)
            {
                collector = collector.Where(e => CheckContains(e, root.Parameter, root.Value));
            }
            
            foreach (Element element in collector)
            {
                elements.Add(CreateElement(element));
            }
            return elements;
        }

        static private bool CheckContains(Element element, string parameter, string value)
        {


            //Debug.WriteLine($"checking {element.Id.IntegerValue} parameter {parameter} contains {value}");
            Parameter param = element.LookupParameter(parameter);
            if (param!=null)
            {
                string paramValue = param.AsValueString();
                if (paramValue!=null)
                {
                    if (paramValue.Contains(value))
                    {
                        return true;
                    }

                }
                //else { Debug.WriteLine($"parameter {parameter} has no value"); }
          
            }
            //else { Debug.WriteLine($"parameter {parameter} does not exist"); }
            //Debug.WriteLine("false");
            return false;
        }

        static private CalcElement CreateElement(Element revitElem)
            ///creats a directuslca element from a revit object
        {
            ParameterSet parameters = revitElem.Parameters;
            Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();
            foreach (Parameter parameter in parameters)
            {
                string name = parameter.Definition.Name;
                if (!parameterDictionary.ContainsKey(name))
                {
                      parameterDictionary.Add(name, parameter.AsValueString());
                }
                
            }
            return new CalcElement(revitElem.Id.ToString(), parameterDictionary);
        }

    }
}
