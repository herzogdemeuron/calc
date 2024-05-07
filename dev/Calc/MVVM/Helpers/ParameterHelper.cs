using System;
using System.Collections.Generic;

namespace Calc.MVVM.Helpers
{
    public class ParameterHelper
    {
     
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
        public static List<string> ValidateParameterNames(List<string> parameterNames)
        {
            // check if the parameter names are legal
            // if yes, return the parameter names
            // this is used before creating the calc elements
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
    }
}
