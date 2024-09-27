

namespace Calc.Core.Color
{
    public struct HslColor
    {
        public readonly int H;
        public readonly int S;
        public readonly int L;

        public HslColor(int hue, int saturation, int lightness)
        {
            this.H = hue;
            this.S = saturation;
            this.L = lightness;
        }
    }
    public struct RgbColor
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;

        public RgbColor(byte red, byte green, byte blue)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
        }
    }

    public struct HexColor
    {
        public readonly string Hex;

        public HexColor(string hex)
        {
            this.Hex = hex;
        }
    }
}
