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

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public int StatusCode;
        public Dictionary<string, object> Response;
        public string Token { get { return _token; } }
        public string Url { get { return _url; } }
        public string GraphQlUrl { get { return $"{_url}/graphql"; } }
        public bool Authenticated { get; set; }
        public HttpClient HttpClient;
        public GraphQLHttpClient Client;
        public GraphQLHttpClient SystemClient { get; private set; }
        private string _token;
        private string _url;
        private string _refreshToken;

        public Directus()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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

            var response = new HttpResponseMessage();
            try
            {
                response = await httpClient.PostAsync($"{this._url}/auth/login", content);
            }
            catch (Exception e)
            {
                this.Authenticated = false;
                Debug.WriteLine($"Authentication aborted, error: {e.Message}");
                return;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Authentication response: {responseContent}");
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
            var response = await this.HttpClient.PostAsync($"{this.Url}/files", content);
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

    internal class DirectusFileUploadResponse
    {
        public DirectusFile Data { get; set; }
    }

    internal class DirectusFile
    {
        public string Id { get; set; } // The UUID of the uploaded file
    }
}