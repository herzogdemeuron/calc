using System;
using System.Collections.Generic;

namespace Calc.Core.Color
{
    public class ColorGradient
    {
        private readonly List<HslColor> _hslColors;
        public List<HslColor> HslColors => this._hslColors;
        public int Saturation { get; set; } = 40;
        public int Lightness { get; set; } = 60;
        public int StartAngle { get; set; } = 80;
        private float e = 2.718f;

        public ColorGradient(int colorCount)
        {
            // create upper limit using sigmoid function
            var ePowerCount = Math.Pow(this.e, colorCount);
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

