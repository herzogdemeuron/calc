using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool CompareBuildups( List<Buildup> buildups1, List<Buildup> buildups2)
        {
            List<int> id1 = buildups1.Select(x => x.Id).OrderBy(x => x).ToList();
            List<int> id2 = buildups2.Select(x => x.Id).OrderBy(x => x).ToList();
            return id1.SequenceEqual(id2);

        }   

    }
}
