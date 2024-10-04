using Calc.MVVM.Models;
using System.Collections;

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
