using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Calc.Core.TestIntegration
{
    [TestClass]
    public class DirectusApiTests
    {
        private string? _email;
        private string? _password;
        private string? _url;

        [TestMethod]
        public async Task Directus_Authenticate_AuthenticatedIsTrue()
        {
            //Arrange
            // get email, password and url from environment variables
            _email = Environment.GetEnvironmentVariable("CALC_DIRECTUS_EMAIL");
            _password = Environment.GetEnvironmentVariable("CALC_DIRECTUS_PASSWORD");
            _url = Environment.GetEnvironmentVariable("CALC_DIRECTUS_URL");
            Console.WriteLine($"email: {_email}");
            Console.WriteLine($"password: {_password}");    
            Console.WriteLine($"url: {_url}");

            // Act
            var directus = new Directus();
            await directus.Authenticate(_url, _email, _password);

            // Assert
            Assert.IsTrue(directus.Authenticated);
            Assert.IsTrue(directus.Token.GetType() == typeof(string));
            Console.WriteLine($"token: {directus.Token}");
        }
    }
}

