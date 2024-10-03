using System;
using System.Collections.Generic;

namespace Calc.Core.Color
{
    /// <summary>
    /// The color gradient created by the amount of colors.
    /// </summary>
    public class ColorGradient
    {
        private readonly List<HslColor> _hslColors;
        public List<HslColor> HslColors => this._hslColors;
        public int Saturation { get; set; } = 30;
        public int Lightness { get; set; } = 70;
        public int StartAngle { get; set; } = 80;

        public ColorGradient(int colorCount)
        {
            // create upper limit using sigmoid function
            var ePowerCount = Math.Pow(2.718f, colorCount);
            var angle = ePowerCount / (ePowerCount + 1);
            // shift to 0 and strech to 1
            angle = (angle - 0.5) * 2;
            // remap to 360 degrees
            angle *= 360;
            this._hslColors = new List<HslColor>();
            var hueStep = angle / colorCount;
            for (int i = 0; i < colorCount; i++)
            {
                var hue = ((int)hueStep * i) + this.StartAngle;
                this._hslColors.Add(new HslColor(hue, this.Saturation, this.Lightness));
            }
        }
    }
}

