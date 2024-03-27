using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.GraphNodes;

namespace Calc.Core.Color
{
    public class ItemPainter
    {
        public static HslColor DefaultColor = new HslColor(0, 0, 86);
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
            var allBranches = new List<IColorizable>();
            foreach (var branch in branches)
            {
                allBranches.AddRange(branch.Flatten());
            }

            SetByIdentifier(allBranches);
        }

        public static void ColorLayersByMaterial(ObservableCollection<BuildupComponent> bcompos)
        {
            var allLayers = new List<IColorizable>();
            foreach (var bcompo in bcompos)
            {
                allLayers.AddRange(bcompo.LayerComponents);
            }

            SetByIdentifier(allLayers);
        }

        private static void SetByIdentifier(List<IColorizable> items)
        {
            var uniqueItems = items.Where(b => !string.IsNullOrEmpty(b.ColorIdentifier))
                                                .Select(b => b.ColorIdentifier).Distinct().ToList();

            var colorGradient = new ColorGradient(uniqueItems.Count);

            foreach (var item in items)
            {
                var buildupIndex = uniqueItems.IndexOf(item.ColorIdentifier);
                if (buildupIndex >= 0)
                {
                    item.HslColor = colorGradient.HslColors[buildupIndex];
                }
                else
                {
                    item.HslColor = DefaultColor;
                }
            }
        }
    }
}
