using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Security.Policy;
using System.Collections.Generic;
using GraphQL.Client.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc.Core.IntegrationTests
{
    [TestClass]
    public class DirectusApiTests
    {
        public readonly string _configPath = "../../directusApiConfig.json";
        private string _token;
        private string _url;

        [TestInitialize]
        public void Initialize()
        {
            SetEnvironmentVariables();
        }

        [TestMethod]
        public void Directus_DefaultConstructor_AreEqualToConfigFile()
        {
            // Act
            var directus = new Directus();

            // Assert
            Assert.AreEqual(_token, directus.Token);
            Assert.AreEqual(_url, directus.Url);
            Assert.IsNotNull(directus.HttpClient);
            Assert.IsNotNull(directus.Client);
            Assert.IsInstanceOfType(directus.HttpClient, typeof(HttpClient));
            Assert.IsInstanceOfType(directus.Client, typeof(GraphQLHttpClient));
            Assert.AreEqual(new Uri(_url), directus.Client.Options.EndPoint);
            Assert.AreEqual("application/json", directus.HttpClient.DefaultRequestHeaders.Accept.First().MediaType);
            Assert.AreEqual("Bearer", directus.HttpClient.DefaultRequestHeaders.Authorization.Scheme);
            Assert.AreEqual(_token, directus.HttpClient.DefaultRequestHeaders.Authorization.Parameter);
        }

        public void SetEnvironmentVariables()
        {
            //open json file and load config
            var config = System.IO.File.ReadAllText(this._configPath);
            var configJson = JsonSerializer.Deserialize<Dictionary<string, string>>(config);

            Environment.SetEnvironmentVariable("DIRECTUS_LCA_TOKEN", configJson["token"], EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("DIRECTUS_LCA_URL", configJson["url"], EnvironmentVariableTarget.Process);

            this._token = configJson["token"];
            this._url = configJson["url"];
        }
    }
}

