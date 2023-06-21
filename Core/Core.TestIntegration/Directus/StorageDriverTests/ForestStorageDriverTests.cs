using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectusLca.StorageDrivers;
using DirectusLca.Filtering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DirectusLca.IntegrationTests
{
    [TestClass]
    public class ForestStorageDriverTests
    {
        // WARNING: set the treeSetId to an existing tree set id in your directus project
        private int forestId = 5;
        private ForestStorageDriver driver;

        [TestInitialize]
        public void Initialize()
        {
            var test = new DirectusApiTests();
            test.SetEnvironmentVariables();
            driver = new ForestStorageDriver();

        }

        [TestMethod]
        public async Task SaveForest_WithProject_ReturnIdGreaterZero()
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

            // Act
            var response = await this.driver.SaveForestToDirectus(forest);

            // Assert
            Assert.IsTrue(response.Id > 0);
            Console.WriteLine(response.Id);
            forestId = response.Id;
        }

        [TestMethod]
        public async Task UpdateForest_WithoutProject_ReturnIdGreaterZero()
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

            // Act
            var response = await this.driver.UpdateForestInDirectus(forest);

            // Assert
            Assert.IsTrue(response.Id > 0);
            Console.WriteLine(response.Id);
        }

        [TestMethod]
        public async Task GetAllForests_Default_ReturnCountGreaterZero()
        {
            // Act
            var response = await this.driver.GetAllForestsFromDirectus();

            // Assert
            Assert.IsTrue(response.Count > 0);
        }
    }
}
