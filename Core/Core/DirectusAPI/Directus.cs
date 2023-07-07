﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using GraphQL.Client.Http;
using Calc.Core.GraphQL.Serializer;
using System.Threading.Tasks;
using GraphQL;
using Speckle.Newtonsoft.Json;

namespace Calc.Core.DirectusAPI
{
    public class Directus
    {
        public string JsonResponse { get; set; }
        public int StatusCode;
        public Dictionary<string, object> Response;
        public string Token { get { return _token; } }
        public string Url { get { return _url; } }
        public bool Authenticated { get; set; }
        public HttpClient HttpClient;
        public GraphQLHttpClient Client;
        private string _token;
        private string _url;
        private string _refreshToken;


        //public Directus(string configPath = "")
        //{
        //    var settings = ConfigLoader.Load(configPath);
        //    // check if variables are in settings
        //    if (!settings.ContainsKey("DIRECTUS_URL") || !settings.ContainsKey("DIRECTUS_TOKEN"))
        //    {
        //        throw new Exception("DIRECTUS_URL or DIRECTUS_TOKEN not found in config.json");
        //    }
        //    this._url = settings["DIRECTUS_URL"];
        //    this._token = settings["DIRECTUS_TOKEN"];

        //    // configure http client
        //    this.HttpClient = new HttpClient();
        //    this.HttpClient.DefaultRequestHeaders.Accept.Clear();
        //    this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._token);

        //    this.Client = new GraphQLHttpClient(new GraphQLHttpClientOptions
        //    {
        //        EndPoint = new Uri(this._url)
        //    }, new NewtonsoftJsonSerializer(), this.HttpClient);
        //}
        public Directus(string url)
        {
            this._url = url;
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

        public async Task Authenticate(string email, string password)
        {
            HttpClient httpClient = new();
            FormUrlEncodedContent content = new (
                new Dictionary<string, string> {
                    { "email", email },
                    { "password", password }
                });
            var response = await httpClient.PostAsync($"{this._url}/auth/login", content);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));

            this._token = response.Data.access_token;
            this._refreshToken = response.Data.refresh_token;
            if (this.Token == null)
            {
                throw new Exception("Authentication failed");
            } else {
            this.Authenticated = true;
            }
        }
    }

    internal class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}