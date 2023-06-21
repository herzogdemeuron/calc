

namespace Calc.Core.Color
{
    public struct HslColor
    {
        public readonly int Hue;
        public readonly int Saturation;
        public readonly int Lightness;

        public HslColor(int hue, int saturation, int lightness)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Lightness = lightness;
        }
    }
    public struct RgbColor
    {
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        public RgbColor(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
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
