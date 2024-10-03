using Calc.Core.Objects.BasicParameters;
using System.Collections.Generic;
using System.Linq;

namespace Calc.Core.Helpers
{
    internal static class ParameterErrorHelper
    {
        /// <summary>
        /// Merges parameter errors, appending element ids
        /// </summary>
        internal static List<ParameterError> MergeParameterErrors(List<List<ParameterError>> errorLists)
        {
            List<ParameterError> result = new();
            foreach (var errorList in errorLists)
            {
                foreach (var error in errorList)
                {
                    var errorCopy = new ParameterError(error);
                    AddToErrorList(result, errorCopy);
                }
            }
            return result;
        }

        /// <summary>
        /// Add a new parameter error to the error list
        /// </summary>
        internal static void AddToErrorList(List<ParameterError> errorList, ParameterError newError)
        {
            var existingError = errorList.FirstOrDefault(x => x.ParameterName == newError.ParameterName && x.ErrorType == newError.ErrorType);
            if (existingError == null)
            {
                errorList.Add(newError);
            }
            else
            {
                var newUniqueIds = newError.ElementIds.Except(existingError.ElementIds).ToList();
                existingError.ElementIds.AddRange(newUniqueIds);
            }
        }
    }
}
