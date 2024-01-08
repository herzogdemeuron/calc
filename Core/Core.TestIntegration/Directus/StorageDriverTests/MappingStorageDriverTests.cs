using Calc.Core.Objects;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Mappings;

namespace Calc.Core.TestIntegration.Drivers
{

    [TestClass]
    public class MappingStorageDriverTests
    {
        private readonly int mappingId = 3;
        private Directus? directus;
        private MockData? mockData;
        private Forest? forest;

        [TestInitialize]
        public async Task Initialize()
        {
            this.directus = await TestUtils.GetAuthenticatedDirectus();
            this.mockData = new MockData();
            this.forest = new Forest() { Trees = mockData.Trees };
            foreach (var tree in this.forest.Trees)
            {
                tree.Plant(mockData.Elements);
                //mockData.AssignBuildups(tree);
            }
        }

        [TestMethod]
        public async Task SaveMappingToDirectus_NoProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.forest, "test mapping name");
            var directusManager = new DirectusManager<Mapping>(this.directus);

            // Act
            var response = await directusManager.CreateSingle<MappingStorageDriver>(new MappingStorageDriver() { SendItem = mapping});
            // Assert
            Assert.IsInstanceOfType(response.CreatedItem, typeof(Mapping));
            Assert.IsInstanceOfType(response.CreatedItem.Id, typeof(int));
        }

        [TestMethod]
        public async Task SaveMappingToDirectus_WithProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.forest, "test mapping name with project")
            {
                Project = new Project { Id = 1 }
            };
            var directusManager = new DirectusManager<Mapping>(this.directus);

            // Act
            var response = await directusManager.CreateSingle<MappingStorageDriver>(new MappingStorageDriver() { SendItem = mapping });

            // Assert
            Assert.IsInstanceOfType(response.CreatedItem, typeof(Mapping));
            Assert.IsInstanceOfType(response.CreatedItem.Id, typeof(int));
        }

        [TestMethod]
        public async Task UpdateMappingInDirectus_NoProject_IsIdResponse()
        {
            // Arrange
            var mapping = new Mapping(this.forest, "updated mapping name")
            {
                Id = mappingId
            };
            var directusManager = new DirectusManager<Mapping>(this.directus);

            // Act
            var response = await directusManager.UpdateSingle<MappingStorageDriver>(new MappingStorageDriver() { SendItem = mapping });

            // Assert
            Assert.IsInstanceOfType(response.UpdatedItem, typeof(Mapping));
            Assert.IsInstanceOfType(response.UpdatedItem.Id, typeof(int));
        }

        [TestMethod]
        public async Task GetMappings_ByName_IsListOfMappings()
        {
            // Arrange
            var directusManager = new DirectusManager<Mapping>(this.directus);

            // Act
            var response = await directusManager.GetMany<MappingStorageDriver>(new MappingStorageDriver());

            // Assert
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<Mapping>));
            var mapping = response.GotManyItems.First();
            Assert.IsInstanceOfType(mapping.Id, typeof(int));
            Assert.IsInstanceOfType(mapping.Name, typeof(string));
            Assert.IsInstanceOfType(mapping.MappingItems, typeof(List<MappingItem>));
        }
    }
}
