using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects;
using Calc.Core.StorageDrivers;

namespace Calc.Core.IntegrationTests
{
    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void Plant_DefaultScenario_IsNotNull()
        {
            // Arrange
            var mockData = new MockData();

            // Act
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                Console.WriteLine("Tree planted");

                // Assert
                Assert.IsNotNull(tree.Elements);
                foreach (var element in tree.Elements)
                {
                   // loop over element fields and write to console
                   foreach (var field in element.Fields)
                    {
                       Console.WriteLine(field);
                   }
                }
            }
        }

        [TestMethod]
        public void GrowBranches_DefaultScenario_IsNotNull()
        {
            // Arrange
            var mockData = new MockData();

            // Act
            foreach (var tree in mockData.Trees)
            {
                tree.Plant(mockData.Elements);
                Console.WriteLine(tree.Name);
                foreach (var param in tree.BranchConfig)
                {
                    Console.WriteLine(param);
                }
                tree.GrowBranches();

                // Assert
                Assert.IsNotNull(tree.SubBranches);
                tree.PrintTree();
            }
        }
    }

    [TestClass]
    public class MappingsTests
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
            }
        }

        [TestMethod]
        public async Task ApplyMappingToTree_Default_TreeEqualsMockdata()
        {
            {
                // WARINING: This test requires the Buildup Ids and Names in the mockdata to match some buildups in the database.
                
                // Arrange

                // copy the trees so we can compare them later
                var treesCopy = new List<Tree>(this.trees);
                foreach (var tree in treesCopy)
                {
                    mockData.AssignBuildups(tree);
                }

                var mappings = await driver.GetAllMappingsFromDirectus();
                // pretty print mappings
                var mappingJson = JsonSerializer.Serialize(mappings, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(mappingJson);

                var buildupStorageDriver = new BuildupStorageDriver();
                var buildups = await buildupStorageDriver.GetAllBuildupsFromDirectus();
                var selectedMapping = mappings.SingleOrDefault(m => m.Id == mappingId);

                // Act
                foreach (var tree in trees)
                {
                    selectedMapping.ApplyMappingToTree(tree, buildups);
                    tree.PrintTree();
                }

                // Check the equality of Buildup.Name for trees
                Assert.IsTrue(trees.Select(t => t.Buildup?.Name).SequenceEqual(treesCopy.Select(t => t.Buildup?.Name)));

                // Check the equality of Buildup.Name for sub-branches
                var subBranches = trees.SelectMany(t => t.SubBranches);
                var subBranchesCopy = treesCopy.SelectMany(t => t.SubBranches);
                Assert.IsTrue(subBranches.Select(sb => sb.Buildup?.Name).SequenceEqual(subBranchesCopy.Select(sb => sb.Buildup?.Name)));

            }
        }
    }
}
