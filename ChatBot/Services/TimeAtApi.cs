using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChatBot.Services
{
    public class TimeAtApi
    {
        private const string TIME_API = "TIME_API";

        private string BASE_URL { get; }

        public TimeAtApi()
        {
            this.BASE_URL = ConfigurationManager.AppSettings[TIME_API];
        }

        public object GetTimeBy(string timezone)
        {
            HttpClient client = this.GetHttpClient();
            client.BaseAddress = new Uri(this.BASE_URL);

            // List data response.
            HttpResponseMessage response = client.GetAsync(timezone).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<dynamic>().Result; 
            }

            return null;
        }

        public void GetAll()
        {
            HttpClient client = this.GetHttpClient();

            // List data response.
            HttpResponseMessage response = client.GetAsync(this.BASE_URL).Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<dynamic>>().Result;
            }
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        } 
    }
}