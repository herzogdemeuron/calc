using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc.Core.TestIntegration
{
    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void Plant_DefaultScenario_IsNotNull()
        {
            // Arrange
            var mockData = new MockData();

            // Act
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                Console.WriteLine("Tree planted");

                // Assert
                Assert.IsNotNull(tree.Elements);
                foreach (var element in tree.Elements)
                {
                   // loop over element fields and write to console
                   foreach (var field in element.Fields)
                    {
                       Console.WriteLine(field);
                   }
                }

                Assert.IsNotNull(tree.SubBranches);
                tree.PrintTree();
            }
        }
    }
}
