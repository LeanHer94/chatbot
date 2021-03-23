using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChatBot.Services
{
    public class TimeAtApi
    {
        private const string TIMEAPI = "TIME_API";

        private string URL { get; }

        public TimeAtApi()
        {
            this.URL = ConfigurationManager.AppSettings[TIMEAPI];
        }

        public void GetTimeBy(string timezone)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(timezone).Result;
        }
    }
}