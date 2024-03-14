using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.IO;
using System.Net;


namespace Calc.DirectusTest
{
    [TestClass]
    public class TlsSupportTests
    {
        [TestMethod]
        public void TestTls13Support()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13; // this is added since directus announced that they force using TLS 1.3 for directus cloud on all traffics

            Assert.IsTrue(IsTls13Supported(), "TLS 1.3 is not supported on this system.");
        }

        private bool IsTls13Supported()
        {
            var supportsTls13 = false;

            // Use a known host that supports TLS 1.3
            var host = "www.example.com"; // Replace with a known TLS 1.3 supported host
            var port = 443; // Standard HTTPS port

            try
            {
                using (var client = new TcpClient(host, port))
                using (var sslStream = new SslStream(client.GetStream()))
                {
                    // Attempt to establish an SSL/TLS connection using only TLS 1.3
                    sslStream.AuthenticateAsClient(host, null, SslProtocols.Tls13, checkCertificateRevocation: true);

                    // If the connection is successful, check if the negotiated protocol is TLS 1.3
                    if (sslStream.SslProtocol == SslProtocols.Tls13)
                    {
                        supportsTls13 = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log exception if needed
                // An exception here likely means TLS 1.3 is not supported
                Console.WriteLine($"Error trying to establish TLS 1.3 connection: {ex.Message}");
            }

            return supportsTls13;
        }
    }
}
