using System.Collections.Generic;
using System.Linq;
using Calc.Core.Objects.GraphNodes;

namespace Calc.Core.Color
{
    /// <summary>
    /// Calculates HSL colors for branches based on their GWP values.
    /// Colors range from dark purple (low GWP) to green (high GWP).
    /// Branches with null GWP (no assemblies) get default gray color.
    /// </summary>
    public class GwpColorMaker
    {
        private readonly HslColor _defaultColor = ItemPainter.DefaultColor;
        private readonly int _purpleHue = 270; // Dark purple
        private readonly int _greenHue = 120;  // Green
        private readonly int _saturation = 70;
        private readonly int _lightness = 50;

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
        /// </summary>
        private HslColor MakeHslColorForGwp(double gwp, double minGwp, double gwpRange)
        {
            int hue;
            
            if (gwpRange == 0)
            {
                // All GWP values are the same, use middle color (magenta - between purple and green)
                hue = 300; // Magenta
            }
            else
            {
                // Calculate position in range (0 to 1)
                var normalizedPosition = (gwp - minGwp) / gwpRange;
                
                // Interpolate hue from dark purple (270) to green (120)
                // Going from purple -> blue -> cyan -> green (counter-clockwise on color wheel)
                // This maps 270° -> 240° -> 180° -> 120°
                hue = (int)(_purpleHue - normalizedPosition * (_purpleHue - _greenHue));
            }
            
            return new HslColor(hue, _saturation, _lightness);
        }

    }
}