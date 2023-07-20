using System;

namespace Calc.Core.Color
{
    public class CalcColorConverter
    {
        public static RgbColor HslToRgb(HslColor hsl)
        {
            var h = hsl.H / 360.0;
            var s = hsl.S / 100.0;
            var l = hsl.L / 100.0;

            double r;
            double g;
            double b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                var hue2Rgb = new Func<double, double, double, double>((x, y, t) =>
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1.0 / 6.0) return x + (y - x) * 6 * t;
                    if (t < 1.0 / 2.0) return y;
                    if (t < 2.0 / 3.0) return x + (y - x) * (2.0 / 3.0 - t) * 6;
                    return x;
                });

                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                var p = 2 * l - q;
                r = hue2Rgb(p, q, h + 1.0 / 3.0);
                g = hue2Rgb(p, q, h);
                b = hue2Rgb(p, q, h - 1.0 / 3.0);
            }

            return new RgbColor((byte)Math.Round(r * 255), (byte)Math.Round(g * 255), (byte)Math.Round(b * 255));
        }

        public static HexColor RgbToHex(RgbColor rgb)
        {
            var r = rgb.R.ToString("X2");
            var g = rgb.G.ToString("X2");
            var b = rgb.B.ToString("X2");
            return new HexColor($"#{r}{g}{b}");
        }
    }
}
