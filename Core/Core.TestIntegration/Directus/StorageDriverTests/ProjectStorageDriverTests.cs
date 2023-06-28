using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Calc.Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration.Drivers
{
    [TestClass]
    public class ProjectStorageDriverTests
    {
        private Directus? directus;

        [TestInitialize]
        public void Initialize()
        {
            this.directus = new Directus(DirectusApiTests.ConfigPath);
        }

        [TestMethod]
        public async Task GetAllProjects_Default_SpecifyLater()
        {
            var storageManager = new DirectusManager<Project>(this.directus);

            // Act
            var response = await storageManager.GetMany<ProjectStorageDriver>(new ProjectStorageDriver());

            // Assert
            Assert.IsNotNull(response.GotManyItems);
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<Project>));
            Assert.IsTrue(response.GotManyItems.Count > 0);

            // serialize buildups to console using System.Text.Json, indent
            var buildupsJson = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(buildupsJson);

        }
    }
}

