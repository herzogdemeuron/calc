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
            if (itemX?.TagName == "?" && itemY?.TagName == "?") return 0;
            if (itemX?.TagName == "?") return 1;
            if (itemY?.TagName == "?") return -1;

            // Compare two non-null/non-empty strings
            return string.Compare(itemX.TagName, itemY.TagName);
        }
    }
}
