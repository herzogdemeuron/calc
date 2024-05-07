using Calc.Core.Objects;
using Calc.Core.Calculations;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Results;

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

            var results = Calculator.Calculate(branches);
            var snapshot = new Snapshot
            { 
                Results = results,
                Project = new Project() { Id = 1 },
                Name = "Test"
            };

            var storageManager = new DirectusManager<Snapshot>(this.directus);

            // Act
            var response = await storageManager.CreateSingle<SnapshotStorageDriver>(new SnapshotStorageDriver() { SendItem = snapshot });
            Console.WriteLine(response.CreatedItem.Id);

            // Assert
            Assert.IsTrue(response.CreatedItem.Id > 0);
        }
    }
}
