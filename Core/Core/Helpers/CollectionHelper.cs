using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Helpers
{
    public static class CollectionHelper
    {

        public static Dictionary<string, int> MergeCountDicts (List<Dictionary<string, int>> dicts)
        {
            Dictionary<string, int> result = new();

            foreach (var dict in dicts)
            {
                foreach (var pair in dict)
                {
                    AddToCountDict(result, pair.Key);
                }
            }

            return result;
        }

        public static void AddToCountDict(Dictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += 1;
            }
            else
            {
                dict.Add(key, 1);
            }
        }
    }
}
