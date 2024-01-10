﻿using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Calc.Core.Helpers
{
    public static class ParameterErrorHelper
    {
        public static ObservableCollection<ParameterError> MergeParameterErrors(List<ObservableCollection<ParameterError>> errorLists)
        {
            ObservableCollection<ParameterError> result = new();
            foreach (var errorList in errorLists)
            {
                foreach (var error in errorList)
                {
                    AddToErrorList(result, error);
                }
            }
            return result;
        }

        /// <summary>
        /// Add a new parameter error to the error list
        /// </summary>
        public static void AddToErrorList(ObservableCollection<ParameterError> errorList, ParameterError newError)
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
