using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using GraphQL.Client.Http;
using Calc.Core.GraphQL.Serializer;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public string JsonResponse { get; set; }
        public int StatusCode;
        public Dictionary<string, object> Response;
        public string Token { get { return _token; } }
        public string Url { get { return _url; } }
        public HttpClient HttpClient;
        public GraphQLHttpClient Client;
        private readonly string _token;
        private readonly string _url;

        public Directus()
        {
            var settings = new ConfigLoader().Load();
            // check if variables are in settings
            if (!settings.ContainsKey("DIRECTUS_URL") || !settings.ContainsKey("DIRECTUS_TOKEN"))
            {
                throw new Exception("DIRECTUS_URL or DIRECTUS_TOKEN not found in config.json");
            }
            this._url = settings["DIRECTUS_URL"];
            this._token = settings["DIRECTUS_TOKEN"];

            // configure http client
            this.HttpClient = new HttpClient();
            this.HttpClient.DefaultRequestHeaders.Accept.Clear();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._token);

            this.Client = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(this._url)
            }, new NewtonsoftJsonSerializer(), this.HttpClient);
        }
    }
}