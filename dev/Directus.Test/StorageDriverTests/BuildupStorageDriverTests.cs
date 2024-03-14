using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.Buildups;
using System.Threading.Tasks;
using CCalc.DirectusTest.StorageDriverTests;
using System.Collections.Generic;
using System.Text.Json;
using System;
using Calc.Core.Calculations;
using Calc.Core.Objects;
using Calc.Core.Objects.Materials;
using GraphQL.Client.Http;
using Polly;

namespace Calc.DirectusTest.StorageDriverTests
{


    [TestClass]
    public class BuildupStorageDriverTests
    {
        private Directus directus;
        private readonly Polly.Retry.AsyncRetryPolicy _graphqlRetry = Policy.Handle<GraphQLHttpRequestException>()
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {

                    if (retryCount == 4)
                    {
                        Environment.Exit(1);
                    }
                });


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

           /* var matstorageManager = new DirectusManager<Material>(this.directus);
            var matresponse = await matstorageManager.GetMany<MaterialStorageDriver>(new MaterialStorageDriver());
            var materialsJson = System.Text.Json.JsonSerializer.Serialize(matresponse, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });*/
            //Console.WriteLine(materialsJson);
        }

        [TestMethod]
        public async Task CreateSingleBuildup()
        {
            var storageManager = new DirectusManager<Buildup>(this.directus);
            var buildup = MockBuildup();
            var buildupDriver = new BuildupStorageDriver();
            buildupDriver = await _graphqlRetry.ExecuteAsync(() =>
                    storageManager.GetMany<BuildupStorageDriver>(buildupDriver));

            buildupDriver.SendItem = buildup;

            var response = await storageManager.CreateSingle<BuildupStorageDriver>(buildupDriver);
            Assert.IsNotNull(response.CreatedItem);
        }

        private static Buildup MockBuildup()
        {
            return new Buildup
            {
               /* Name = "TestBuildup",
                Standard = new LcaStandard { Id = 1 },
                BuildupUnit = Core.Objects.Unit.m,
                Description = "TestDescription ccc",
                CalculationComponents = new List<CalculationComponent>
                {
                    new CalculationComponent
                    {
                        Function = "TestFunction",
                        Quantity = 1,
                        GWP = 1,
                        GE = 1,
                        Material = new Material { Id = 7 }
                    }
                }*/
            };
        }
    }
}
