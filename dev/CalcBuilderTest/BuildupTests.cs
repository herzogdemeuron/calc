using CalcBuilderTest;
using CalcBuilderTest.MockData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CalcTest
{
    [TestClass]
    public class BuildupTests
    {
        [TestMethod]
        public void BuildupTest()
        {
            var buildups = MockBuildups.GetBuildups();
            var buildup = buildups[0];
            var layerCompos = MockBuildupComponents.GetLayerComponents();
            buildup.ApplyBuildupComponents(layerCompos);
            Assert.AreEqual(layerCompos.Count, buildup.Components.Count);

        }
    }
}
