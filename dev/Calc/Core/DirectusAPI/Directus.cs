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
        public static HttpClient HttpClient = new HttpClient(); // Singleton HttpClient
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

        static Directus()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Authenticate(string url, string email, string password)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                this.Authenticated = false;
                throw new ArgumentException("Invalid input.");
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                this.Authenticated = false;
                throw new ArgumentException("Invalid URL.");
            }

            this._url = url;
            string requestBody = $"{{\"email\": \"{email}\", \"password\": \"{password}\"}}";
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await SendRequestWithTokenRefresh(() => HttpClient.PostAsync($"{this._url}/auth/login", content));

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, List<LoginErrorResponse>>>(responseContent);
                this.Authenticated = false;
                string message = errorResponse.Values.FirstOrDefault()?.FirstOrDefault()?.Message ?? "Authentication failed";
                throw new ApplicationException(message);
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
            }
            else
            {
                this.Authenticated = false;
                Debug.WriteLine("Authentication failed");
                throw new ApplicationException("Authentication failed");
            }
        }

        private async Task RefreshToken()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { refresh_token = _refreshToken }), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync($"{this._url}/auth/refresh", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to refresh token: {errorResponse}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);
            this._token = responseData["data"].Access_token;
            this._refreshToken = responseData["data"].Refresh_token;

            ConfigureHttpClient();  // Reconfigure HttpClient to use new token
        }

        public async Task<HttpResponseMessage> SendRequestWithTokenRefresh(Func<Task<HttpResponseMessage>> httpRequest)
        {
            var response = await httpRequest();
            if (response.StatusCode == HttpStatusCode.Unauthorized)  // Check if unauthorized due to expired token
            {
                await RefreshToken();
                response = await httpRequest();  // Retry the request with the new token
            }
            return response;
        }

        private void ConfigureHttpClient()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);
            this.Client = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(this.GraphQlUrl)
            }, new NewtonsoftJsonSerializer(), HttpClient);

            this.SystemClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri($"{this.Url}/graphql/system")
            }, new NewtonsoftJsonSerializer(), HttpClient);

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
            HttpClient.PostAsync($"{this.Url}/files", content));

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