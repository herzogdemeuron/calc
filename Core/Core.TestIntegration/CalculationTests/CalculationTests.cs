using Calc.Core.Objects;
using Calc.Core.Calculations;

namespace Calc.Core.IntegrationTests
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
                tree.GrowBranches();
                mockData.AssignBuildups(tree);
                tree.RemoveElementsByBuildupOverrides();
                branches.AddRange(tree.Flatten());
            }
            foreach (var branch in branches)
            {
                Console.WriteLine(branch.Buildup);
            }

            // Act
            var results = GwpCalculator.CalculateGwp(branches);

            // Assert
            foreach (var result in results)
            {
                Console.WriteLine(result.GlobalWarmingPotentialA1A2A3);
                var expected = mockData.gwp123 * mockData.Amount * mockData.Area;
                Assert.AreEqual(expected, result.GlobalWarmingPotentialA1A2A3);
            }
        }
    }
}
