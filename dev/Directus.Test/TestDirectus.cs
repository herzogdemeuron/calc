using Calc.Core.DirectusAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Calc.DirectusTest.StorageDriverTests;

namespace Calc.DirectusTest
{
    [TestClass]
    public class DirectusApiTests
    {
        private string _email;
        private string _password;
        private string _url;

        [TestMethod]
        public async Task Directus_Authenticate_AuthenticatedIsTrue()
        {
            /*AppContext.SetSwitch("Switch.System.Net.DontEnableSchUseStrongCrypto", true);
            AppContext.SetSwitch("Switch.System.Net.DontEnableSystemDefaultTlsVersions", true);
            AppContext.SetSwitch("Switch.System.ServiceModel.DisableUsingServicePointManagerSecurityProtocols", false);
            AppContext.SetSwitch("Switch.System.ServiceModel.DontEnableSystemDefaultTlsVersions", true);*/


            //Arrange
            // get email, password and url from environment variables
            _email = Environment.GetEnvironmentVariable("CALC_DIRECTUS_EMAIL");
            _password = Environment.GetEnvironmentVariable("CALC_DIRECTUS_PASSWORD");
            _url = Environment.GetEnvironmentVariable("CALC_DIRECTUS_URL");
          /*  _url = "http://localhost:8055";
            _password = "d1r3ctu5";
            _email = "admin@example.com";*/
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

