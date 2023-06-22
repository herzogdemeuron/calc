namespace Calc.Core.IntegrationTests
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
            }
        }

        [TestMethod]
        public void GrowBranches_DefaultScenario_IsNotNull()
        {
            // Arrange
            var mockData = new MockData();

            // Act
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                Console.WriteLine(tree.Name);
                foreach (var param in tree.BranchConfig)
                {
                    Console.WriteLine(param);
                }
                tree.GrowBranches();

                // Assert
                Assert.IsNotNull(tree.SubBranches);
                tree.PrintTree();
            }
        }
    }
}
