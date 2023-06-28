using Calc.Core.Color;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration.ColorTests
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void TestColorGradient()
        {
            // Arrange

            // Act
            var colorGradient = new ColorGradient(10);
            var hslColors = colorGradient.HslColors;

            // Assert
            Assert.AreEqual(10, hslColors.Count);
        }

        [TestMethod]
        public void TestHslToRgb()
        {
            // Arrange
            // make a red color
            var hslColor = new HslColor(0, 100, 50);

            // Act
            var rgbColor = ColorConverter.HslToRgb(hslColor);

            // Assert
            Assert.AreEqual(255, rgbColor.Red);
            Assert.AreEqual(0, rgbColor.Green);
            Assert.AreEqual(0, rgbColor.Blue);
        }

        [TestMethod]
        public void TestRgbToHex()
        {
            // Arrange
            var rgbColor = new RgbColor(255, 0, 0);

            // Act
            var hexColor = ColorConverter.RgbToHex(rgbColor);

            // Assert
            Assert.AreEqual("#FF0000", hexColor.Hex);
        }
    }
}
