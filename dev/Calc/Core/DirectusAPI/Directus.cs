﻿using System;
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
    /// <summary>
    /// The core class for interacting with Directus API with retry policies.
    /// Both HttpClient (auth and image) and GraphQLHttpClient are used for making requests to Directus API.
    /// </summary>
    public class Directus
    {
        private string token ;
        private string baseUrl;
        private string refreshToken;

        private string reEmail;
        private string rePassword;
        public bool Authenticated { get; private set; } = false;
        private readonly HttpClient httpClient;
        private GraphQLHttpClient graphQlClient;
        private GraphQLHttpClient graphQlSysClient;

        private AsyncRetryPolicy graphQlRetryPolicy = 
            Policy.Handle<GraphQLHttpRequestException>()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        private AsyncRetryPolicy httpRetryPolicy =
            Policy.Handle<HttpRequestException>()
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));


        public Directus()
        {
            httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // unless windows supports TLS 1.3, directus cloud enforces TLS 1.3
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> RequestWithRetry(string url, Func<HttpContent> contentFactory, bool reAuth = true)
        {
            var action = new Func<Task<HttpResponseMessage>>(() => httpClient.PostAsync(url, contentFactory()));
            var response = await httpRetryPolicy.ExecuteAsync(action);

            // re-authenticate and retry the request if the response is "Unauthorized"
            if (response.StatusCode == HttpStatusCode.Unauthorized && reAuth)
            {
                // waite for sevaral seconds before retrying
                await Task.Delay(5000);
                await RefreshAuthentication();
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

        public async Task<GraphQLResponse<TDriver>> GraphQlQueryWithRetry<TDriver>(GraphQLRequest request)
        {
            var response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlClient.SendQueryAsync<TDriver>(request));
            bool isUnauthorized = response.Errors != null && response.Errors.Any(e => e.Message.Contains("Unauthorized"));
            if (isUnauthorized)
            {
                await RefreshAuthentication();
                response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlClient.SendQueryAsync<TDriver>(request));
            }
            if (response.Errors != null && response.Data == null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response;

        }

        public async Task<GraphQLResponse<TDriver>> GraphQlMutationWithRetry<TDriver>(GraphQLRequest request)
        {
            var response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlClient.SendMutationAsync<TDriver>(request));
            bool isUnauthorized = response.Errors != null && response.Errors.Any(e => e.Message.Contains("Unauthorized"));
            if (isUnauthorized)
            {
                await RefreshAuthentication();
                response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlClient.SendQueryAsync<TDriver>(request));
            }
            if (response.Errors != null && response.Data == null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response;
        }
        

        public async Task<GraphQLResponse<TDriver>> GraphQlSysQueryWithRetry<TDriver>(GraphQLRequest request)
        {
            var response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlSysClient.SendQueryAsync<TDriver>(request));
            bool isUnauthorized = response.Errors != null && response.Errors.Any(e => e.Message.Contains("Unauthorized"));
            if (isUnauthorized)
            {
                await RefreshAuthentication();
                response = await graphQlRetryPolicy.ExecuteAsync(async () => await graphQlClient.SendQueryAsync<TDriver>(request));
            }
            if (response.Errors != null && response.Data == null)
            {
                throw new Exception(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }

            return response;
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
            reEmail = email;
            rePassword = password;
            ConfigureGraphQlClients();
            Authenticated = true;

        }

        public async Task RefreshAuthentication()
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidOperationException("Refresh token is missing.");
            }

            string requestBody = JsonConvert.SerializeObject(new { refresh_token = refreshToken });
            var contentFactory = () => new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await RequestWithRetry($"{baseUrl}/auth/refresh", contentFactory, false);
            // if the refresh token is expired, re-authenticate
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Task.Delay(5000);
                await Authenticate(baseUrl, reEmail, rePassword);
                return;
            }
            var responseContent = await ReadResponseContent(response);
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, LoginResponseData>>(responseContent);
            LoginResponseData data = responseData["data"];

            token = data.Access_token;
            refreshToken = data.Refresh_token;
            ConfigureGraphQlClients();
            Authenticated = true;
        }


        private void ConfigureGraphQlClients()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            graphQlClient = new GraphQLHttpClient
                (
                new GraphQLHttpClientOptions { EndPoint = new Uri($"{this.baseUrl}/graphql") },
                new NewtonsoftJsonSerializer(),
                httpClient
                );

            graphQlSysClient = new GraphQLHttpClient
                (
                new GraphQLHttpClientOptions { EndPoint = new Uri($"{this.baseUrl}/graphql/system") },
                new NewtonsoftJsonSerializer(),
                httpClient
                );
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