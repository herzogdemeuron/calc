using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DirectusLca.StorageDrivers;
using DirectusLca.Calculations;

namespace DirectusLca.IntegrationTests
{
    [TestClass]
    public class ResultStorageDriverTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var test = new DirectusApiTests();
            test.SetEnvironmentVariables();
        }

        [TestMethod]
        public async Task SaveResultsToDirectus_NoSnapshot_SpecifyLater()
        {
            // Arrange
            var mockData = new MockData();
            var branches = new List<Filtering.Branch>();
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
            var ids = await ResultStorageDriver.SaveResultsToDirectus(results);
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
            var branches = new List<Filtering.Branch>();
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
            var ids = ResultStorageDriver.SaveResultsToDirectus(results, snapshot);

            // Assert
            Assert.IsNotNull(ids);
        }
    }
}
