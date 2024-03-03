using CalcBuilderTest;
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

        }
    }
}
