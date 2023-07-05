using Calc.Core.Objects;
using Calc.Core.Calculations;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class ResultStorageDriverTests
    {
        private Directus? directus;

        [TestInitialize]
        public void Initialize()
        {
            this.directus = new Directus(DirectusApiTests.ConfigPath);
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
                mockData.AssignBuildups(tree);
                tree.RemoveElementsByBuildupOverrides();
                branches.AddRange(tree.Flatten());
            }

            var results = Calculator.Calculate(branches);
            foreach (var result in results)
            {
                result.SnapshotName = "test snapshot name";
                result.Project = new Project { Id = 1 };
            }
            var storageManager = new DirectusManager<Result>(this.directus);

            // Act
            var response = await storageManager.CreateMany<ResultStorageDriver>(new ResultStorageDriver() { SendItems = results });
            foreach (var result in response.CreatedManyItems)
            { Console.WriteLine(result.Id); }

            // Assert
            Assert.IsTrue(response.CreatedManyItems.Count > 0);
        }
    }
}
