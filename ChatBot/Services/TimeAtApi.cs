using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChatBot.Services
{
    public class TimeAtApi
    {
        private string URL { get; }

        public TimeAtApi()
        {
            this.URL = "";
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