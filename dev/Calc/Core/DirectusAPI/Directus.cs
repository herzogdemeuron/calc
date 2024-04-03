using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using GraphQL.Client.Http;
using Calc.Core.GraphQL.Serializer;
using System.Threading.Tasks;
using Speckle.Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Linq;
using Polly;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public int StatusCode;
        public Dictionary<string, object> Response;
        private string _token;
        public string Token { get { return _token; } }
        private string _url;
        public string Url { get { return _url; } }
        public string GraphQlUrl { get { return $"{_url}/graphql"; } }
        public bool Authenticated { get; set; }
        public HttpClient HttpClient;
        public GraphQLHttpClient Client;
        public GraphQLHttpClient SystemClient { get; private set; }
        private string _refreshToken;

        private readonly IAsyncPolicy<HttpResponseMessage> _httpRetryPolicy =
                        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) 
                              .Or<HttpRequestException>()
                              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry: (outcome, timespan, retryAttempt, context) =>
                                {
                                    Console.WriteLine($"Retry {retryAttempt} due to {outcome.Exception?.Message ?? "HTTP " + outcome.Result.StatusCode}");
                                });

        public Directus()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<string> Authenticate(string url, string email, string password)
        {
            if ( string.IsNullOrEmpty(url) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) )
            {
                this.Authenticated = false;
                return "Please provide complete details.";
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                this.Authenticated = false;
                return "Invalid URL.";
            }

            this._url = url;

            string requestBody = $"{{\"email\": \"{email}\", \"password\": \"{password}\"}}";
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = new HttpResponseMessage();
            try
            {
                response = await _httpRetryPolicy.ExecuteAsync(() =>
                httpClient.PostAsync($"{this._url}/auth/login", content));
            }
            catch (Exception e)
            {
                this.Authenticated = false;
                return e.Message;
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject <Dictionary<string, List<LoginErrorResponse>>>(responseContent);
                this.Authenticated = false;
                return errorResponse.Values.FirstOrDefault()?.FirstOrDefault()?.Message ?? "Authentication failed";
            }

            var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);
            LoginResponseData data = responseData["data"];

            this._token = data.Access_token;
            this._refreshToken = data.Refresh_token;

            if (this.Token != null)
            {
                this.Authenticated = true;
                Debug.WriteLine("Authentication successful");
                ConfigureHttpClient();
                return null;
            }
            else
            {
                this.Authenticated = false;
                Debug.WriteLine("Authentication failed");
                return "Authentication failed";
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

            this.SystemClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri($"{this.Url}/graphql/system")
            }, new NewtonsoftJsonSerializer(), this.HttpClient);

            Debug.WriteLine("HttpClient configured");
        }

        public async Task<string> UploadImageAsync(string imagePath, string folderId, string newFileName)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.", imagePath);
            }

            // Prepare the multipart/form-data request
            var content = new MultipartFormDataContent();
            var imageContent = new StreamContent(File.OpenRead(imagePath));
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            // Specify the filename in the Content-Disposition header
            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = $"\"{newFileName}\""
            };

            if (folderId != null)
            {
                content.Add(new StringContent(folderId), "folder");
            }
            content.Add(imageContent, "file", Path.GetFileName(imagePath));

            // Send the request to the Directus file upload endpoint
            HttpResponseMessage response = await _httpRetryPolicy.ExecuteAsync(() =>
            this.HttpClient.PostAsync($"{this.Url}/files", content));

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to upload image: {errorResponse}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var uploadResponse = JsonConvert.DeserializeObject<DirectusFileUploadResponse>(responseContent);
            return uploadResponse?.Data?.Id; // Return the UUID of the uploaded image
        }
    }

    internal class LoginResponseData
    {
        public string Access_token { get; set; }
        public int Expires { get; set; }
        public string Refresh_token { get; set; }
    }

    internal class LoginErrorResponse
    {
        public string Message { get; set; }
    }

    internal class DirectusFileUploadResponse
    {
        public DirectusFile Data { get; set; }
    }

    internal class DirectusFile
    {
        public string Id { get; set; } // The UUID of the uploaded file
    }
}