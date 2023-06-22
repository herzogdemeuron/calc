using Calc.Core.Objects;
using Calc.Core.Calculations;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.Core.DirectusAPI;

namespace Calc.Core.IntegrationTests
{
    [TestClass]
    public class ResultStorageDriverTests
    {
        private ResultStorageDriver driver;

        [TestInitialize]
        public void Initialize()
        {
            var directus = new Directus(DirectusApiTests.ConfigPath);
            this.driver = new ResultStorageDriver(directus);
        }

        [TestMethod]
        public async Task SaveResultsToDirectus_NoSnapshot_SpecifyLater()
        {
            // Arrange
            var mockData = new MockData();
            var branches = new List<Branch>();
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                tree.GrowBranches();
                mockData.AssignBuildups(tree);
                tree.RemoveElementsByBuildupOverrides();
                branches.AddRange(tree.Flatten());
            }
           
            var results = GwpCalculator.CalculateGwp(branches);

            // Act
            var ids = await driver.SaveResultsToDirectus(results);
            foreach (var id in ids)
            {  Console.WriteLine(id); }

            // Assert
            Assert.IsNotNull(ids);
        }
 
        [TestMethod]
        public void SaveResultsToDirectus_WithSnapshot_SpecifyLater()
        {
            // Arrange
            var mockData = new MockData();
            var branches = new List<Branch>();
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                tree.GrowBranches();
                mockData.AssignBuildups(tree);
                tree.RemoveElementsByBuildupOverrides();
                branches.AddRange(tree.Flatten());
            }
            foreach (var branch in branches)
            {
                Console.WriteLine(branch.Buildup);
            }
            var results = GwpCalculator.CalculateGwp(branches);
            var snapshot = new Snapshot
            {
                Name = "Test",
                Project = new Project
                { ProjectNumber = "test project" },
            };

            // Act
            var ids = driver.SaveResultsToDirectus(results, snapshot);

            // Assert
            Assert.IsNotNull(ids);
        }
    }
}
