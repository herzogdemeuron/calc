using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Calc.Core.Helpers
{
    public static class ParameterErrorHelper
    {
        public static List<ParameterError> MergeParameterErrors(List<List<ParameterError>> errorLists)
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
        public static void AddToErrorList(List<ParameterError> errorList, ParameterError newError)
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
