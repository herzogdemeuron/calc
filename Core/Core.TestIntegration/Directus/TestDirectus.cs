using GraphQL.Client.Http;
using Calc.Core.DirectusAPI;

namespace Calc.Core.TestIntegration
{
    [TestClass]
    public class DirectusApiTests
    {
        public static readonly string ConfigPath = "Directus/config.json";
        private string? _token;
        private string? _url;

        [TestMethod]
        public void Directus_DefaultConstructor_AreEqualToConfigFile()
        {
            //Arrange
            // print current working directory
            Console.WriteLine(Directory.GetCurrentDirectory());
            var settings = ConfigLoader.Load(ConfigPath);
            this._token = settings["DIRECTUS_TOKEN"];
            this._url = settings["DIRECTUS_URL"];

            // Act
            var directus = new Directus(ConfigPath);

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
    }
}

