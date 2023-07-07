using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using GraphQL.Client.Http;
using Calc.Core.GraphQL.Serializer;
using System.Threading.Tasks;
using GraphQL;
using Speckle.Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public string JsonResponse { get; set; }
        public int StatusCode;
        public Dictionary<string, object> Response;
        public string Token { get { return _token; } }
        public string Url { get { return _url; } }
        public string GraphQlUrl { get { return $"{_url}/graphql"; } }
        public bool Authenticated { get; set; }
        public HttpClient HttpClient;
        public GraphQLHttpClient Client;
        private string _token;
        private string _url;
        private string _refreshToken;

        public Directus()
        {
        }

        public async Task Authenticate(string url, string email, string password)
        {
            // if any of the inputs is null, set Authenticated to false and return
            if (url == null || email == null || password == null)
            {
                this.Authenticated = false;
                Debug.WriteLine("Authentication aborted, one or more of the inputs is null");
                return;
            }

            // check if url is valid URI
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                this.Authenticated = false;
                Debug.WriteLine("Authentication aborted, url is not a valid URI");
                return;
            }

            this._url = url;

            string requestBody = $"{{\"email\": \"{email}\", \"password\": \"{password}\"}}";
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync($"{this._url}/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response content into the custom class
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);

            // Access the content of the 'data' field
            LoginResponseData data = responseData["data"];

            this._token = data.Access_token;
            this._refreshToken = data.Refresh_token;

            if (this.Token != null)
            {
                this.Authenticated = true;
                Debug.WriteLine("Authentication successful");
                ConfigureHttpClient();
            }
            else
            {
                this.Authenticated = false;
                Debug.WriteLine("Authentication failed");
            }
        }

        private void ConfigureHttpClient()
        {
            this.HttpClient = new HttpClient();
            this.HttpClient.DefaultRequestHeaders.Accept.Clear();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);

            this.Client = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(this.GraphQlUrl)
            }, new NewtonsoftJsonSerializer(), this.HttpClient);
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