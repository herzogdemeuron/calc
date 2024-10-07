using System;
using System.Collections.Generic;

namespace Calc.MVVM.Helpers
{
    internal class ParameterHelper
    {
        /// <summary>
        /// Checks if the parameter names from query template are legal,
        /// In the query template:
        /// type paramter name: "type: parameterName",
        /// instance parameter name: "inst: parameterName".
        /// </summary>
        /// <returns>true/false for instance/type parameter, and the parameter name string. If illegal, return null string.</returns>
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
        /// Validate and filter the parameter names.
        /// </summary>
        /// <returns>The valid parameter names.</returns>
        internal static List<string> ValidateParameterNames(List<string> parameterNames)
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
