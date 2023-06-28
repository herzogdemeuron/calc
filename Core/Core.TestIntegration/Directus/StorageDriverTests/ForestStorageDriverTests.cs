using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class ForestStorageDriverTests
    {
        // WARNING: set the treeSetId to an existing tree set id in your directus project
        private readonly int forestId = 10;
        private Directus? directus;

        [TestInitialize]
        public void Initialize()
        {
            this.directus = new Directus(DirectusApiTests.ConfigPath);
        }

        [TestMethod]
        public async Task SaveForest_WithProject_ReturnValidId()
        {
              // Arrange
            var mockData = new MockData();
            var trees = mockData.Trees;
            foreach (var tree in trees)
            {
                tree.Plant(mockData.Elements);
            }
            var forest = new Forest
            {
                Name = "test forest",
                Trees = trees,
                Project = new Project { Id = 1 }
            };
            var storageManager = new DirectusManager<Forest>(this.directus);

            // Act
            var response = await storageManager.CreateSingle<ForestStorageDriver>(new ForestStorageDriver() { SendItem = forest });

            // Assert
            Assert.IsTrue(response.CreatedItem.Id > 0);
            Console.WriteLine(response.CreatedItem.Id);
        }

        [TestMethod]
        public async Task UpdateForest_WithProject_ReturnValidId()
        {
            // Arrange
            var mockData = new MockData();
            var trees = mockData.Trees;
            foreach (var tree in trees)
            {
                tree.Plant(mockData.Elements);
            }
            var forest = new Forest
            {
                Name = "updated forest",
                Trees = trees,
                Project = new Project { Id = 1 },
                Id = this.forestId
            };
            var storageManager = new DirectusManager<Forest>(this.directus);

            // Act
            var response = await storageManager.UpdateSingle<ForestStorageDriver>(new ForestStorageDriver() { SendItem = forest });

            // Assert
            Assert.IsTrue(response.UpdatedItem.Id > 0);
            Console.WriteLine(response.UpdatedItem.Id);
        }

        [TestMethod]
        public async Task GetAllForests_Default_ReturnMultipleItems()
        {
            var storageManager = new DirectusManager<Forest>(this.directus);
            // Act
            var response = await storageManager.GetMany<ForestStorageDriver>(new ForestStorageDriver());

            // Assert
            Assert.IsTrue(response.GotManyItems.Count > 0);
        }
    }
}
