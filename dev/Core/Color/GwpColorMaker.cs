using System.Collections.Generic;
using System.Linq;
using Calc.Core.Objects.GraphNodes;

namespace Calc.Core.Color
{
    /// <summary>
    /// Calculates HSL colors for branches based on their GWP values.
    /// Colors range from light cyan #7BD5F5 (low GWP) to light purple #787FF6 (high GWP).
    /// Branches with null GWP (no assemblies) get default gray color.
    /// </summary>
    public class GwpColorMaker
    {
        private readonly HslColor _defaultColor = ItemPainter.DefaultColor;
        private readonly HslColor _lowGwpColor = new HslColor(194, 84, 73); // #7BD5F5 - light cyan
        private readonly HslColor _highGwpColor = new HslColor(243, 87, 73); // #787FF6 - light purple

        /// <summary>
        /// Assigns HSL colors to branches based on their GWP values relative to the current branch group.
        /// Works like ColorBranchesByBranch but uses GWP values for color calculation.
        /// </summary>
        /// <param name="branches">The list of branches at the current level to color</param>
        public void AssignGwpBasedColorsToLevel(List<Branch> branches)
        {
            if (branches == null || branches.Count == 0)
                return;

            // Get GWP values from the current level branches only
            var gwpValues = branches
                .Select(b => b.Gwp)
                .Where(gwp => gwp.HasValue)
                .Select(gwp => gwp.Value)
                .ToList();

            if (gwpValues.Count == 0)
            {
                // No GWP values found, assign default color to all
                foreach (var branch in branches)
                {
                    branch.HslColor = _defaultColor;
                }
                return;
            }

            var minGwp = gwpValues.Min();
            var maxGwp = gwpValues.Max();
            var gwpRange = maxGwp - minGwp;

            // Create a lookup for GWP to color mapping to ensure same GWP values get same color
            var gwpToColorMap = new Dictionary<double, HslColor>();

            foreach (var branch in branches)
            {
                if (!branch.Gwp.HasValue)
                {
                    // No assembly assigned, use default gray color
                    branch.HslColor = _defaultColor;
                }
                else
                {
                    var gwp = branch.Gwp.Value;
                    
                    // Check if we already calculated color for this GWP value
                    if (!gwpToColorMap.ContainsKey(gwp))
                    {
                        gwpToColorMap[gwp] = MakeHslColorForGwp(gwp, minGwp, gwpRange);
                    }
                    
                    branch.HslColor = gwpToColorMap[gwp];
                }
            }
        }


        /// <summary>
        /// Calculates HSL color for a specific GWP value within the given range.
        /// Interpolates between light cyan (low GWP) and light purple (high GWP).
        /// </summary>
        private HslColor MakeHslColorForGwp(double gwp, double minGwp, double gwpRange)
        {
            if (gwpRange == 0)
            {
                // All GWP values are the same, use middle color between low and high
                var midHue = (_lowGwpColor.H + _highGwpColor.H) / 2;
                var midSaturation = (_lowGwpColor.S + _highGwpColor.S) / 2;
                var midLightness = (_lowGwpColor.L + _highGwpColor.L) / 2;
                return new HslColor(midHue, midSaturation, midLightness);
            }
            
            // Calculate position in range (0 to 1)
            var normalizedPosition = (gwp - minGwp) / gwpRange;
            
            // Interpolate between low GWP color and high GWP color
            var hue = (int)(_lowGwpColor.H + normalizedPosition * (_highGwpColor.H - _lowGwpColor.H));
            var saturation = (int)(_lowGwpColor.S + normalizedPosition * (_highGwpColor.S - _lowGwpColor.S));
            var lightness = (int)(_lowGwpColor.L + normalizedPosition * (_highGwpColor.L - _lowGwpColor.L));
            
            return new HslColor(hue, saturation, lightness);
        }

    }
}