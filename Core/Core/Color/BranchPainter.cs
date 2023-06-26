using System;
using System.Collections.Generic;
using System.Text;
using Speckle.Newtonsoft.Json;
using Calc.Core.Objects;
using System.Linq;

namespace Calc.Core.Color
{
    public class BranchPainter
    {
        public static void ColorBranchesByBranch(List<Branch> branches)
        {
            if (branches.Count == 0)
            {
                Console.WriteLine("No branches to color.");
                return;
            }

            var colorGradient = new ColorGradient(branches.Count);
            for (int index = 0; index < branches.Count; index++)
            {
                branches[index].HslColor = colorGradient.HslColors[index];
                ColorBranchesByBranch(branches[index].SubBranches);
            }
        }

        public static void ColorBranchesByBuildup(List<Branch> branches)
        {
            var flattenedBranches = new List<Branch>();
            foreach (var branch in branches)
            {
                flattenedBranches.AddRange(branch.Flatten());
            }

            var uniqueBuildups = flattenedBranches.Select(b => b.Buildup).Distinct().ToList();
            var colorGradient = new ColorGradient(uniqueBuildups.Count);
            var defaultColor = new HslColor(0, 0, 100);

            for (int i = 0; i < flattenedBranches.Count; i++)
            {
                var branch = flattenedBranches[i];
                var buildupIndex = uniqueBuildups.IndexOf(branch.Buildup);
                if (buildupIndex >= 0)
                {
                    branch.HslColor = colorGradient.HslColors[buildupIndex];
                }
                else
                {
                    branch.HslColor = defaultColor;
                }
            }
        }
    }
}
