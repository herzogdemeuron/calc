using Speckle.Newtonsoft.Json;
using Calc.Core.Color;
using Calc.Core.Objects;

namespace Calc.Core.TestIntegration.ColorTests
{
    [TestClass]
    public class BranchColorizerTests
    {
        List<Tree>? Trees { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockData = new MockData();

            // Act
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
               
            }
            this.Trees = mockData.Trees;
        }

        [TestMethod]
        public void ColorBranchesByBranch_Default_CreatesColors()
        {
            // Arrange
            // check if trees are not null
            Assert.IsNotNull(this.Trees);

            // Act
            List<Branch> branches = this.Trees.ConvertAll(tree => (Branch)tree);
            BranchColorizer.ColorBranchesByBranch(branches);

            // Assert
            foreach (var tree in this.Trees)
            {
                tree.PrintTree();
                foreach (var branch in tree.Flatten())
                {
                    Assert.IsNotNull(branch.HslColor);
                }
            }
        }

        [TestMethod]
        public void ColorBranchesByProperty_Default_CreatesColors()
        {
            // Arrange
            // check if trees are not null
            Assert.IsNotNull(this.Trees);
            var mockData = new MockData();
            foreach (var tree in this.Trees)
            {
                mockData.AssignBuildups(tree);
            }

            //Console.WriteLine(JsonConvert.SerializeObject(this.Trees, Formatting.Indented));

            // Act
            List<Branch> branches = this.Trees.ConvertAll(tree => (Branch)tree);
            BranchColorizer.ColorBranchesByBuildup(branches);

            // Assert
            foreach (var tree in this.Trees)
            {
                tree.PrintTree();
                foreach (var branch in tree.Flatten())
                {
                    Assert.IsNotNull(branch.HslColor);
                }
            }
        }
    }
}
