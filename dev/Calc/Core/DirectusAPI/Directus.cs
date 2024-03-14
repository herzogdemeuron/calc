using System;
using System.Collections.Generic;
using GraphQL.Client.Http;
using Calc.Core.GraphQL.Serializer;
using System.Threading.Tasks;
using Speckle.Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public int StatusCode { get; private set; }
        public Dictionary<string, object> Response { get; private set; }
        public string Token => _token;
        public string Url => _url;
        public string GraphQlUrl => $"{_url}/graphql";
        public bool Authenticated { get; private set; }
        public static HttpClient HttpClient
        {
            get
            {
              /*  var handler = new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls13
                };*/
                return new HttpClient();
            }
        }

        public GraphQLHttpClient Client { get; private set; }

        private string _token;
        private string _url;
        private string _refreshToken;

        public Directus()
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Authenticate(string url, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Authenticated = false;
                Debug.WriteLine("Authentication aborted: One or more of the inputs is null or whitespace.");
                return;
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Authenticated = false;
                Debug.WriteLine("Authentication aborted: URL is not a valid URI.");
                return;
            }

            _url = url;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13; // this is added since directus announced that they force using TLS 1.3 for directus cloud on all traffics

            var requestBody = JsonConvert.SerializeObject(new { email, password });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.PostAsync($"{_url}/auth/login", content);
                StatusCode = (int)response.StatusCode;

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Authentication response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);
                    if (responseData != null && responseData.TryGetValue("data", out LoginResponseData data))
                    {
                        _token = data.Access_token;
                        _refreshToken = data.Refresh_token;

                        if (!string.IsNullOrEmpty(_token))
                        {
                            Authenticated = true;
                            Debug.WriteLine("Authentication successful");
                            ConfigureHttpClient();
                            return;
                        }
                    }
                }

                Authenticated = false;
                Debug.WriteLine("Authentication failed: Unable to retrieve token.");
            }
            catch (Exception e)
            {
                Authenticated = false;
                Debug.WriteLine($"Authentication aborted, error: {e.Message}");
            }
        }

        private void ConfigureHttpClient()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            Client = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(GraphQlUrl)
            }, new NewtonsoftJsonSerializer(), HttpClient);
            Debug.WriteLine("HttpClient configured");
        }
    }

    internal class LoginResponseData
    {
        public string Access_token { get; set; }
        public int Expires { get; set; }
        public string Refresh_token { get; set; }
    }
}