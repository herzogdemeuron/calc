using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DirectusLca.Filtering;
using DirectusLca.StorageDrivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DirectusLca.IntegrationTests
{

    [TestClass]
    public class MappingStorageDriverTests
    {
        private readonly int mappingId = 29;
        private MappingStorageDriver driver;
        private MockData mockData;
        private List<Tree> trees;

        [TestInitialize]
        public void Initialize()
        {
            var test = new DirectusApiTests();
            test.SetEnvironmentVariables();
            this.driver = new MappingStorageDriver();
            this.mockData = new MockData();
            this.trees = mockData.Trees;
            foreach (var tree in this.trees)
            {
                tree.Plant(mockData.Elements);
                tree.GrowBranches();
                mockData.AssignBuildups(tree);
            }
        }

        [TestMethod]
        public async Task SaveMappingToDirectus_NoProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.trees, "test mapping name");

            // Act
            var response = await this.driver.SaveMappingToDirectus(mapping);

            // Assert
            Assert.IsInstanceOfType(response, typeof(IdResponse));
            Assert.IsInstanceOfType(response.Id, typeof(int));

            // Bit of a shitty practice here. Making two tests would be cleaner,
            // but I don't have the time to research that rigt now.

            ////Act
            // var response2 = await MappingStorageDriver.SaveMappingToDire(
            //       trees,
            //       "updated name",
            //       mappingIdInDatabase: response.Id);

            //// Assert
            //Assert.IsInstanceOfType(response2, typeof(IdResponse));
            //Assert.IsInstanceOfType(response2.Id, typeof(int));
        }

        [TestMethod]
        public async Task SaveMappingToDirectus_WithProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.trees, "test mapping name with project")
            {
                Project = new Project { Id = 1 }
            };

            // Act
            var response = await this.driver.SaveMappingToDirectus(mapping);

            // Assert
            Assert.IsInstanceOfType(response, typeof(IdResponse));
            Assert.IsInstanceOfType(response.Id, typeof(int));
        }

        [TestMethod]
        public async Task UpdateMappingInDirectus_NoProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.trees, "updated mapping name")
            {
                Id = mappingId
            };

            // Act
            var response = await this.driver.UpdateMappingInDirectus(mapping);

            // Assert
            Assert.IsInstanceOfType(response, typeof(IdResponse));
            Assert.IsInstanceOfType(response.Id, typeof(int));
        }

        [TestMethod]
        public async Task GetMappings_ByName_IsListOfMappings()
        {
            // Arrange

            // Act
            var mappings = await this.driver.GetAllMappingsFromDirectus();

            // Assert
            Assert.IsInstanceOfType(mappings, typeof(List<Mapping>));
            var mapping = mappings.First();
            Assert.IsInstanceOfType(mapping.Id, typeof(int));
            Assert.IsInstanceOfType(mapping.Name, typeof(string));
            Assert.IsInstanceOfType(mapping.MappingItems, typeof(List<MappingItem>));
        }
    }
}
