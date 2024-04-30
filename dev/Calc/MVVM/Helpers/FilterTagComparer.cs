using Calc.MVVM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.MVVM.Helpers
{
    public class FilterTagComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var itemX = x as FilterTagModel;
            var itemY = y as FilterTagModel;

            // Handle nulls last
            if (string.IsNullOrEmpty(itemX?.Family) && string.IsNullOrEmpty(itemY?.Family)) return 0;
            if (string.IsNullOrEmpty(itemX?.Family)) return 1;
            if (string.IsNullOrEmpty(itemY?.Family)) return -1;

            // Compare two non-null/non-empty strings
            return string.Compare(itemX.Family, itemY.Family);
        }
    }
}
