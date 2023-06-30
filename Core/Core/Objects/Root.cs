using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speckle.Newtonsoft.Json;

namespace Calc.Core.Objects
{
    public class Root
    {
        [JsonIgnore]
        public List<CalcElement> Elements { get; set; }
        [JsonIgnore]
        public List<string> ElementIds => Elements.Select(e => e.Id).ToList();
        public string Parameter { get; set; }
        public string Method { get; set; }
        public string Value { get; set; }
        [JsonIgnore]
        public Dictionary<string, Action<object[]>> FilterMethods { get; set; }

        public Root()
        /// <summary>
        /// Helper constructor to access all the filter methods.
        /// Use "GetFilterMethods()" to get a list of all the methods.
        /// </summary>
        {
            SetFilterMethods();
        }

        public Root(string parameterName, string methodName, string value) : this()
        /// <summary>
        /// This is the acutal constructor to create a root filter.
        /// Use this constructor to create a root the is used in a tree.
        /// </summary>
        {
            Parameter = parameterName;
            Method = methodName;
            Value = value;
        }

        public void SetFilterMethods()
        {
            FilterMethods = new Dictionary<string, Action<object[]>>
            {
                { "Parameter Contains Value", args => ParameterContainsValue() },
                // Add other methods here
            };
        }

        public List<string> GetFilterMethods()
        {
            return FilterMethods.Keys.ToList();
        }

        public void CallFilterMethod(string methodKey, params object[] arguments)
        {
            if (FilterMethods.TryGetValue(methodKey, out Action<object[]> method))
            {
                method.Invoke(arguments);
            }
            else
            {
                throw new ArgumentException("Invalid filter method.");
            }
        }

        public void ParameterContainsValue()
        {
            var filteredElements = new List<CalcElement>();
            foreach (var element in Elements)
            {
                // check if parameter is in element fields
                if (!element.Fields.ContainsKey(Parameter))
                {
                    continue;
                }
                // check if parameter value contains value
                if (element.Fields[Parameter].ToString().Contains(Value))
                {
                    filteredElements.Add(element);
                }
            }
            Elements = filteredElements;
        }

        public string Serialize()
        {
            var json = new StringBuilder();
            json.Append("{");
            json.Append($"\"Parameter\": \"{Parameter}\",");
            json.Append($"\"Method\": \"{Method}\",");
            json.Append($"\"Value\": \"{Value}\"");
            json.Append("}");
            return json.ToString();
        }

    }
}
