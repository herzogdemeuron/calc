using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DirectusLca.StorageDrivers;

namespace DirectusLca.IntegrationTests
{
    [TestClass]
    public class BuildupStorageDriverTests
    {
        private BuildupStorageDriver driver;

        [TestInitialize]
        public void Initialize()
        {
            var test = new DirectusApiTests();
            test.SetEnvironmentVariables();
            this.driver = new BuildupStorageDriver();
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
