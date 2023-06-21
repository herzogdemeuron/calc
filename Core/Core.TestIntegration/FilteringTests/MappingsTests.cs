using System.Text.Json;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.Core.DirectusAPI;

namespace Calc.Core.IntegrationTests
{
    [TestClass]
    public class MappingsTests
    {
        private readonly int mappingId = 29;
        private MappingStorageDriver driver;
        private MockData mockData;
        private List<Tree> trees;
        private Directus directus;

        [TestInitialize]
        public void Initialize()
        {
            this.directus = new Directus(DirectusApiTests.ConfigPath);
            this.driver = new MappingStorageDriver(directus);
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

                var buildupStorageDriver = new BuildupStorageDriver(this.directus);
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
