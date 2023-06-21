using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Color;

namespace Calc.Core.IntegrationTests
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

            // Act
            var colorGradient = new ColorGradient(10);
            var rgbColors = new List<RgbColor>();
            foreach (var hslColor in colorGradient.HslColors)
            {
                rgbColors.Add(ColorConverter.HslToRgb(hslColor));
            }
            var rgbColor = rgbColors[0];

            // Assert
            Assert.AreEqual(255, rgbColor.Red);
            Assert.AreEqual(0, rgbColor.Green);
            Assert.AreEqual(0, rgbColor.Blue);
        }

        [TestMethod]
        public void TestRgbToHex()
        {
            // Arrange

            // Act
            var colorGradient = new ColorGradient(10);
            var hexColors = new List<HexColor>();
            foreach (var hslColor in colorGradient.HslColors)
            {
                var rgbColor = ColorConverter.HslToRgb(hslColor);
                hexColors.Add(ColorConverter.RgbToHex(rgbColor));
            }
            var hexColor = hexColors[0];

            // Assert
            Assert.AreEqual("#FF0000", hexColor.Hex);
        }
    }
}
