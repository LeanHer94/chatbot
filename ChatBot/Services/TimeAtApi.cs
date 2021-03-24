using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public string GetTimeBy(string timezone)
        {
            HttpClient client = this.GetHttpClient();
            client.BaseAddress = new Uri(this.BASE_URL);

            // List data response.
            HttpResponseMessage response = client.GetAsync(timezone).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsAsync<WebApiResultDTO>().Result;

                // So Datetime.Parse don't use it to convert to local datetime.
                var offset = 6;
                return result.datetime.Remove(result.datetime.Length - offset, offset);
            }

            return null;
        }

        public IEnumerable<string> GetAll()
        {
            HttpClient client = this.GetHttpClient();

            // List data response.
            HttpResponseMessage response = client.GetAsync(this.BASE_URL).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<string>>().Result;
            }

            return Enumerable.Empty<string>();
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