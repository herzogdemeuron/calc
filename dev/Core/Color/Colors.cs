

namespace Calc.Core.Color
{
    public readonly struct HslColor(int hue, int saturation, int lightness)
    {
        public readonly int H = hue;
        public readonly int S = saturation;
        public readonly int L = lightness;
    }
    public readonly struct RgbColor(byte red, byte green, byte blue)
    {
        public readonly byte R = red;
        public readonly byte G = green;
        public readonly byte B = blue;
    }

    public readonly struct HexColor(string hex)
    {
        public readonly string Hex = hex;
    }
}
