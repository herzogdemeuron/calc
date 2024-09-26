﻿using Calc.Core.DirectusAPI;
using Calc.Core.DirectusAPI.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc.Core.Objects.Assemblies;
using System.Threading.Tasks;
using CCalc.DirectusTest.StorageDriverTests;
using System.Collections.Generic;
using System.Text.Json;
using System;
using Calc.Core.Calculation;
using Calc.Core.Objects;
using Calc.Core.Objects.Materials;
using GraphQL.Client.Http;
using Polly;
using Calc.Core.Objects.Standards;

namespace Calc.DirectusTest.StorageDriverTests
{


    [TestClass]
    public class StorageDriverTests
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
        public async Task GetProjects()
        {
            var storageManager = new DirectusManager<CalcProject>(this.directus);

            // Act
            var response = await storageManager.GetMany<ProjectStorageDriver>(new ProjectStorageDriver());

            // Assert
            Assert.IsNotNull(response.GotManyItems);
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<CalcProject>));
            Assert.IsTrue(response.GotManyItems.Count > 0);

            var json = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);
        }

        [TestMethod]
        public async Task GetAllStandards()
        {
            var storageManager = new DirectusManager<LcaStandard>(this.directus);

            // Act
            var response = await storageManager.GetMany<StandardStorageDriver>(new StandardStorageDriver());

            // Assert
            Assert.IsNotNull(response.GotManyItems);
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<LcaStandard>));
            Assert.IsTrue(response.GotManyItems.Count > 0);

            // serialize assemblies to console using System.Text.Json, indent
            var assembliesJson = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(assembliesJson);
        }

        [TestMethod]
        public async Task GetAllAssemblies_Default_SpecifyLater()
        {
            var storageManager = new DirectusManager<Assembly>(this.directus);

            // Act
            var response = await storageManager.GetMany<AssemblyStorageDriver>(new AssemblyStorageDriver());

            // Assert
            Assert.IsNotNull(response.GotManyItems);
            Assert.IsInstanceOfType(response.GotManyItems, typeof(List<Assembly>));
            Assert.IsTrue(response.GotManyItems.Count > 0);

            // serialize assemblies to console using System.Text.Json, indent
            var assembliesJson = System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(assembliesJson);

           /* var matstorageManager = new DirectusManager<Material>(this.directus);
            var matresponse = await matstorageManager.GetMany<MaterialStorageDriver>(new MaterialStorageDriver());
            var materialsJson = System.Text.Json.JsonSerializer.Serialize(matresponse, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });*/
            //Console.WriteLine(materialsJson);
        }

        [TestMethod]
        public async Task CreateSingleAssembly()
        {
            var storageManager = new DirectusManager<Assembly>(this.directus);
            var assembly = MockAssembly();
            var assemblyDriver = new AssemblyStorageDriver();
            assemblyDriver = await _graphqlRetry.ExecuteAsync(() =>
                    storageManager.GetMany<AssemblyStorageDriver>(assemblyDriver));

            assemblyDriver.SendItem = assembly;

            var response = await storageManager.CreateSingle<AssemblyStorageDriver>(assemblyDriver);
            Assert.IsNotNull(response.CreatedItem);
        }

        private static Assembly MockAssembly()
        {
            return new Assembly
            {
               /* Name = "TestAssembly",
                Standard = new LcaStandard { Id = 1 },
                AssemblyUnit = Core.Objects.Unit.m,
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
