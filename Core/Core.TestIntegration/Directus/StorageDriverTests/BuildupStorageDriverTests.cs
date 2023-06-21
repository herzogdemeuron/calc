using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.StorageDrivers;


namespace Calc.Core.IntegrationTests
{
    [TestClass]
    public class BuildupStorageDriverTests
    {
        private BuildupStorageDriver driver;

        [TestInitialize]
        public void Initialize()
        {
            var directus = new Directus(DirectusApiTests.ConfigPath);
            this.driver = new BuildupStorageDriver(directus);

        }

        [TestMethod]
        public async Task GetAllBuildups_Default_SpecifyLater()
        {
            // Act
            var buildups = await this.driver.GetAllBuildupsFromDirectus();

            // Assert
            Assert.IsNotNull(buildups);
            Assert.IsInstanceOfType(buildups, typeof(List<Buildup>));
            Assert.IsTrue(buildups.Count > 0);

            // serialize buildups to console using System.Text.Json, indent
            var buildupsJson = System.Text.Json.JsonSerializer.Serialize(buildups, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(buildupsJson);

        }
    }
}
