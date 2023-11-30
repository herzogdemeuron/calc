using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Helpers
{
    public static class CollectionHelper
    {

        public static Dictionary<string, List<string>> MergeCountDicts (List<Dictionary<string, List<string>>> dicts)
        {
            Dictionary<string, List<string>> result = new();

            foreach (var dict in dicts)
            {
                foreach (var pair in dict)
                {
                    AddToCountDict(result, pair.Key, pair.Value.ToArray());
                }
            }

            return result;
        }

        public static void AddToCountDict(Dictionary<string, List<string>> dict, string typeName, params string[] elementIds)
        {
            if (!dict.ContainsKey(typeName))
            {
                dict[typeName] = new List<string>();
            }

            dict[typeName].AddRange(elementIds);

        }

    }
}
