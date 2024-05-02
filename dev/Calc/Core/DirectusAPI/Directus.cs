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
using GraphQL;
using Polly.Retry;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        private string token ;
        private string baseUrl;
        private string refreshToken;

        private string reAuthEmail;
        private string reAuthPassword;
        public bool Authenticated { get; private set; } = false;
        private readonly HttpClient httpClient;
        private GraphQLHttpClient graphQlClient;
        private GraphQLHttpClient graphQlSysClient;


        public Directus()
        {
            httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // unless windows supports TLS 1.3, directus cloud enforces TLS 1.3
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private AsyncRetryPolicy CreateHttpRetryPolicy()
        {
            return Policy.Handle<HttpRequestException>()
                         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private AsyncRetryPolicy CreateGraphQLRetryPolicy()
        {
            return Policy.Handle<GraphQLHttpRequestException>()
                         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private void ConfigureHttpClients()
        {
            var uri1 = new Uri(this.baseUrl);
            var uri = new Uri($"{this.baseUrl}/graphql");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            graphQlClient = new GraphQLHttpClient
                (
                new GraphQLHttpClientOptions{ EndPoint = new Uri($"{this.baseUrl}/graphql")}, 
                new NewtonsoftJsonSerializer(), 
                httpClient
                );

            graphQlSysClient = new GraphQLHttpClient
                (
                new GraphQLHttpClientOptions{ EndPoint = new Uri($"{this.baseUrl}/graphql/system") },
                new NewtonsoftJsonSerializer(),
                httpClient
                );
        }

        public async Task<HttpResponseMessage> RequestWithRetry(string url, Func<HttpContent> contentFactory, bool reAuth = true)
        {
            var action = new Func<Task<HttpResponseMessage>>(() => httpClient.PostAsync(url, contentFactory()));
            var httpRetryPolicy = CreateHttpRetryPolicy();
            var response = await httpRetryPolicy.ExecuteAsync(action);

            // re-authenticate and retry the request if the response is "Unauthorized"
            if (response.StatusCode == HttpStatusCode.Unauthorized && reAuth)
            {
                await Authenticate(baseUrl, reAuthEmail, reAuthPassword);
                response = await httpRetryPolicy.ExecuteAsync(action);
            }
            return response;
        }

        public async Task<string> ReadResponseContent(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, List<LoginErrorResponse>>>(responseContent);
                string message = errorResponse.Values.FirstOrDefault()?.FirstOrDefault()?.Message ?? "Requesting to Directus failed";
                throw new ApplicationException(message);
            }

            return responseContent;
        }

        public async Task<T> ExecuteGraphQlRequestWithRetry<T>(Func<Task<GraphQLResponse<T>>> action)
        {
            var retryPolicy = CreateGraphQLRetryPolicy();
            var response = await retryPolicy.ExecuteAsync( () => action());
            // If the response contains an "Unauthorized" error, re-authenticate and retry the request
            if (response.Errors != null && response.Errors.Any(e => e.Message.Contains("Unauthorized")))
            {
                await Authenticate(baseUrl, reAuthEmail, reAuthPassword);
                response = await retryPolicy.ExecuteAsync(action);
            }

            if (response.Errors != null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response.Data;
        }

        public async Task<T> GraphQlQueryWithRetry<T>(GraphQLRequest request)
        {
            return await ExecuteGraphQlRequestWithRetry(() => graphQlClient.SendQueryAsync<T>(request));
        }

        public async Task<T> GraphQlMutationWithRetry<T>(GraphQLRequest request)
        {
            return await ExecuteGraphQlRequestWithRetry(() => graphQlClient.SendMutationAsync<T>(request));
        }

        public async Task<T> GraphQlSysQueryWithRetry<T>(GraphQLRequest request)
        {
            return await ExecuteGraphQlRequestWithRetry(() => graphQlSysClient.SendQueryAsync<T>(request));
        }

        public async Task Authenticate(string url, string email, string password)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Please provide URL, email and password.");
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL.");
            }

            string requestBody = JsonConvert.SerializeObject(new{ email, password });
            var contentFactory = () => new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await RequestWithRetry($"{url}/auth/login", contentFactory, false);
            var responseContent = await ReadResponseContent(response);
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);
            LoginResponseData data = responseData["data"];

            baseUrl = url;
            token = data.Access_token;
            refreshToken = data.Refresh_token;
            ConfigureHttpClients();
            reAuthEmail = email;
            reAuthPassword = password;
            Authenticated = true;

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
            var contentFactory = () => content;
            HttpResponseMessage response = await RequestWithRetry($"{baseUrl}/files", contentFactory);
            var responseContent = await ReadResponseContent(response);
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