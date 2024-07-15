using Calc.Core.Objects;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Snapshots;

namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class SnapshotStorageDriverTests
    {
        private Directus? directus;

        [TestInitialize]
        public async Task Initialize()
        {
            this.directus = await TestUtils.GetAuthenticatedDirectus();
        }

        [TestMethod]
        public async Task SaveResultsToDirectus_NoProject_ReturnsItems()
        {
            // Arrange
            var mockData = new MockData();
            var branches = new List<Branch>();
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                //mockData.AssignBuildups(tree);
                tree.RemoveElementsByBuildupOverrides();
                branches.AddRange(tree.Flatten());
            }

            var results = SnapshotMaker.Calculate(branches);
            var snapshot = new ProjectResult
            { 
                Results = results,
                Project = new CalcProject() { Id = 1 },
                Name = "Test"
            };

            var storageManager = new DirectusManager<ProjectResult>(this.directus);

            // Act
            var response = await storageManager.CreateSingle<ProjectResultStorageDriver>(new ProjectResultStorageDriver() { SendItem = snapshot });
            Console.WriteLine(response.CreatedItem.Id);

            // Assert
            Assert.IsTrue(response.CreatedItem.Id > 0);
        }
    }
}
