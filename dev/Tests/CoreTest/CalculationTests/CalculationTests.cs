using Calc.Core.Objects;
using Calc.Core.Calculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration
{
    [TestClass]
    public class GwpCalculatorTests
    {
        [TestMethod]
        public void Calculate()
        {
            // Arrange
            var mockData = new MockData();

            foreach (var tree in mockData.Forest.Trees)
            {
                tree.Plant(mockData.Elements);
                mockData.Mapping.ApplyToTree(tree,mockData.Assemblies);
            }
            var trees = mockData.Forest.Trees;
            Assert.IsNotNull(trees.SelectMany(t=>t.AssemblySnapshots));
        }
    }
}
