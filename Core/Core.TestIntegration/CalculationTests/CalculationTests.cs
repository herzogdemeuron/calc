using Calc.Core.Objects;
using Calc.Core.Calculations;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration
{
    [TestClass]
    public class GwpCalculatorTests
    {
        [TestMethod]
        public void CalculateGwp_Default_SpecifyLater()
        {
            // Arrange
            var mockData = new MockData();
            var branches = new List<Branch>();
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                mockData.AssignBuildups(tree);
                //tree.RemoveElementsByBuildupOverrides();
                var a = tree;
                //branches.AddRange(tree.Flatten());
            }
            var results = new List<Result>();
            foreach (var branch in branches)
            {
                results.AddRange(branch.CalculationResults);
                Console.WriteLine(branch.CalculationResults);
            }

            // Act
            //var results = Calculator.Calculate(branches);

            // Assert
            foreach (var result in results)
            {
                Console.WriteLine(result.Gwp);
                var expected = mockData.gwp123 * mockData.Amount * mockData.Area;
                Assert.AreEqual(expected, result.Gwp);
            }
        }
    }
}
