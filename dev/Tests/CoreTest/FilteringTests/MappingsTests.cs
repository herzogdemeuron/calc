using System.Text.Json;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.GraphNodes;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Mappings;

namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class MappingsTests
    {
        private readonly int mappingId = 3;
        private Directus? directus;
        private MockData? mockData;
        private List<Tree>? trees;

        [TestInitialize]
        public async Task Initialize()
        {
            this.directus = await TestUtils.GetAuthenticatedDirectus();
            this.mockData = new MockData();
            this.trees = mockData.Trees;

            foreach (var tree in this.trees)
            {
                tree.Plant(mockData.Elements);
            }
        }

        [TestMethod]
        public async Task ApplyMappingToTree_Default_TreeEqualsMockdata()
        {
            {
                // WARINING: This test requires the Buildup Ids and Names in the mockdata to match some assemblies in the database.
                // Arrange
                Assert.IsNotNull(this.trees);
                Assert.IsNotNull(this.mockData);

                // copy the trees so we can compare them later
                var treesCopy = new List<Tree>(this.trees);
                foreach (var tree in treesCopy)
                {
                    //mockData.AssignAssemblies(tree);
                }
                var mappingManager = new DirectusManager<Mapping>(this.directus);
                var response = await mappingManager.GetMany<MappingStorageDriver>(new MappingStorageDriver());
                var mappings = response.GotManyItems;

                // pretty print mappings
                var mappingJson = JsonSerializer.Serialize(mappings, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(mappingJson);
                var assemblyManager = new DirectusManager<Assembly>(this.directus);
                var assemblyResponse = await assemblyManager.GetMany<AssemblyStorageDriver>(new AssemblyStorageDriver());
                var assemblies = assemblyResponse.GotManyItems;

                var selectedMapping = mappings.SingleOrDefault(m => m.Id == mappingId);
                Assert.IsNotNull(selectedMapping);

                // Act
                foreach (var tree in trees)
                {
                    selectedMapping.ApplyToTree(tree, assemblies);
                    tree.PrintTree();
                }

                /*// Check the equality of Buildup.Name for trees
                Assert.IsTrue(trees.Select(t => t.Assemblies?.Name).SequenceEqual(treesCopy.Select(t => t.Assemblies?.Name)));

                // Check the equality of Buildup.Name for sub-branches
                var subBranches = trees.SelectMany(t => t.SubBranches);
                var subBranchesCopy = treesCopy.SelectMany(t => t.SubBranches);
                Assert.IsTrue(subBranches.Select(sb => sb.Assemblies?.Name).SequenceEqual(subBranchesCopy.Select(sb => sb.Assemblies?.Name)));
*/
            }
        }
    }
}
