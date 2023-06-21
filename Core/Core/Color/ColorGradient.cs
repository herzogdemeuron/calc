using System.Collections.Generic;

namespace Calc.Core.Color
{
    public class ColorGradient
    {
        private readonly List<HslColor> _hslColors;
        public List<HslColor> HslColors => this._hslColors;

        public ColorGradient(int colorCount)
        {
            this._hslColors = new List<HslColor>();
            var saturation = 50;
            var lightness = 50;
            var hueStep = 360 / colorCount;
            for (int i = 0; i < colorCount; i++)
            {
                var hue = hueStep * i;
                this._hslColors.Add(new HslColor(hue, saturation, lightness));
            }
        }
    }
}
