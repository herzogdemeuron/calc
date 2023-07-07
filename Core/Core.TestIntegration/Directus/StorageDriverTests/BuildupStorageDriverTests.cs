using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class BuildupStorageDriverTests
    {
        private Directus? directus;

        [TestInitialize]
        public async Task Initialize()
        {
            this.directus = await TestUtils.GetAuthenticatedDirectus();
        }

        [TestMethod]
        public async Task GetAllBuildups_Default_SpecifyLater()
        {
            var storageManager = new DirectusManager<Buildup>(this.directus);

            // Act
            var response = await storageManager.GetMany<BuildupStorageDriver>(new BuildupStorageDriver());

            // Assert
            Assert.IsNotNull(response.GotManyItems);
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<Buildup>));
            Assert.IsTrue(response.GotManyItems.Count > 0);

            // serialize buildups to console using System.Text.Json, indent
            var buildupsJson = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(buildupsJson);

        }
    }
}
